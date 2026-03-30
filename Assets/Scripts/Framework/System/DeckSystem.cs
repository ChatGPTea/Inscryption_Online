using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using QFramework;

namespace Inscryption
{
    /// <summary>
    /// 卡组系统：管理最多三套卡组，支持保存、导出、导入（加密）
    /// </summary>
    public class DeckSystem : InscryptionController
    {
        #region 单例

        public static DeckSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDecks();
                LoadDecks();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region 数据结构

        [System.Serializable]
        public class DeckCardEntry
        {
            public int CardID;
            public int Count;
        }

        [System.Serializable]
        public class DeckData
        {
            public string DeckName;
            public CostType DeckCostType;
            public List<DeckCardEntry> Cards = new List<DeckCardEntry>();
        }

        #endregion

        #region 配置

        [Header("最多三套卡组")]
        public DeckData[] Decks = new DeckData[3];

        [Header("当前编辑的卡组索引")]
        public int EditingDeckIndex = 0;

        // 加密密钥（不要公开）
        private static readonly byte[] CryptoKey = Encoding.UTF8.GetBytes("ver001");

        #endregion

        #region 初始化

        private void InitializeDecks()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Decks[i] == null)
                    Decks[i] = new DeckData { DeckName = $"卡组{i + 1}", DeckCostType = CostType.None };
            }
        }

        #endregion

        #region 保存与加载

        public void SaveDeck(int deckIndex, DeckData data)
        {
            if (deckIndex >= 0 && deckIndex < 3)
            {
                Decks[deckIndex] = data;
                PlayerPrefs.SetString($"Deck_{deckIndex}", JsonUtility.ToJson(data));
                PlayerPrefs.Save();
            }
        }

        public void LoadDecks()
        {
            for (int i = 0; i < 3; i++)
            {
                string json = PlayerPrefs.GetString($"Deck_{i}", "");
                if (!string.IsNullOrEmpty(json))
                    Decks[i] = JsonUtility.FromJson<DeckData>(json);
            }
        }

        #endregion

        #region 加密与解密

        private static byte[] XorEncryptDecrypt(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                result[i] = (byte)(data[i] ^ CryptoKey[i % CryptoKey.Length]);
            return result;
        }

        private static string EncryptCardIDs(List<int> cardIDs)
        {
            byte[] data = new byte[cardIDs.Count * 4];
            for (int i = 0; i < cardIDs.Count; i++)
                BitConverter.GetBytes(cardIDs[i]).CopyTo(data, i * 4);

            data = XorEncryptDecrypt(data);
            return Convert.ToBase64String(data);
        }

        private static List<int> DecryptCardIDs(string encrypted)
        {
            try
            {
                byte[] data = Convert.FromBase64String(encrypted);
                data = XorEncryptDecrypt(data);

                List<int> cardIDs = new List<int>();
                for (int i = 0; i < data.Length; i += 4)
                    cardIDs.Add(BitConverter.ToInt32(data, i));
                return cardIDs;
            }
            catch
            {
                Debug.LogError("卡组代码解密失败！");
                return new List<int>();
            }
        }

        #endregion

        #region 导出与导入

        public string ExportDeckToCode(int deckIndex)
        {
            var deck = Decks[deckIndex];
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"卡组名：{deck.DeckName}");
            sb.AppendLine($"费用类型：{deck.DeckCostType}");
            sb.AppendLine($"卡牌数: {GetTotalCount(deck)}");
            sb.AppendLine();
            sb.AppendLine("复制此卡组代码导入游戏");
            sb.AppendLine("Cards:");

            Dictionary<int, int> cardCounts = new Dictionary<int, int>();
            foreach (var entry in deck.Cards)
            {
                if (cardCounts.ContainsKey(entry.CardID))
                    cardCounts[entry.CardID] += entry.Count;
                else
                    cardCounts[entry.CardID] = entry.Count;
            }

            foreach (var kvp in cardCounts)
            {
                var cardInfo = this.SendQuery(new FindCardInfoWithIDQuery(kvp.Key));
                int cost = cardInfo != null ? cardInfo.Cost : 0;
                string name = cardInfo != null ? cardInfo.Name : $"未知卡牌({kvp.Key})";
                sb.AppendLine($"{kvp.Value}x({cost}) {name}");
            }

            sb.AppendLine();

            List<int> cardIDs = new List<int>();
            foreach (var entry in deck.Cards)
                for (int i = 0; i < entry.Count; i++)
                    cardIDs.Add(entry.CardID);

            sb.Append(EncryptCardIDs(cardIDs));
            return sb.ToString();
        }

        public DeckData ImportDeckFromCode(string code)
        {
            DeckData deck = new DeckData();
            string[] lines = code.Split('\n');
            int state = 0;
            List<int> cardIDs = new List<int>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                if (state == 0)
                {
                    if (line.StartsWith("卡组名：")) deck.DeckName = line.Substring(4);
                    else if (line.StartsWith("CostType：")) deck.DeckCostType = ParseCostType(line.Substring(9));
                    else if (line == "Cards:") state = 1;
                }
                else if (state == 1)
                {
                    if (line == "") state = 2;
                }
                else if (state == 2)
                {
                    cardIDs = DecryptCardIDs(line);
                    break;
                }
            }

            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (int id in cardIDs)
                if (counts.ContainsKey(id)) counts[id]++;
                else counts[id] = 1;

            foreach (var kvp in counts)
                deck.Cards.Add(new DeckCardEntry { CardID = kvp.Key, Count = kvp.Value });

            return deck;
        }

        private int GetTotalCount(DeckData deck)
        {
            int total = 0;
            foreach (var entry in deck.Cards)
                total += entry.Count;
            return total;
        }

        private CostType ParseCostType(string str)
        {
            if (Enum.TryParse(str, out CostType type))
                return type;
            return CostType.None;
        }

        #endregion

        #region 复制到剪贴板

        public void CopyDeckCodeToClipboard(int deckIndex)
        {
            string code = ExportDeckToCode(deckIndex);
            GUIUtility.systemCopyBuffer = code;
            Debug.Log("卡组代码已复制到剪贴板");
        }

        #endregion

        #region 修改牌组

        /// <summary>
        /// 将卡牌加入当前编辑的卡组
        /// </summary>
        public void AddCardToDeck(int cardID)
        {
            var deck = Decks[EditingDeckIndex];
            var entry = deck.Cards.Find(c => c.CardID == cardID);
            if (entry != null)
            {
                entry.Count++;
            }
            else
            {
                deck.Cards.Add(new DeckCardEntry { CardID = cardID, Count = 1 });
            }
            WndManager.Instance.GetWnd<CardListWnd>()?.RefreshDeckDisplay();
            Debug.Log($"已将卡牌 {cardID} 加入卡组 {EditingDeckIndex + 1}");
        }
        /// <summary>
        /// 从当前编辑的卡组中移除一张特定卡牌（减少数量，数量为0时移除条目）
        /// </summary>
        public void RemoveCardFromDeck(int cardID)
        {
            var deck = Decks[EditingDeckIndex];
            var entry = deck.Cards.Find(c => c.CardID == cardID);
            if (entry != null)
            {
                entry.Count--;
                if (entry.Count <= 0)
                {
                    deck.Cards.Remove(entry);
                }
                Debug.Log($"已从卡组 {EditingDeckIndex + 1} 移除一张卡牌 {cardID}");
            }
            else
            {
                Debug.LogWarning($"卡组 {EditingDeckIndex + 1} 中没有卡牌 {cardID}");
            }
            WndManager.Instance.GetWnd<CardListWnd>()?.RefreshDeckDisplay();

        }

        /// <summary>
        /// 清空当前编辑的卡组
        /// </summary>
        [ContextMenu("清空当前卡组")]
        public void ClearCurrentDeck()
        {
            var deck = Decks[EditingDeckIndex];
            deck.Cards.Clear();
            SaveDeck(EditingDeckIndex, deck);
            Debug.Log($"已清空卡组 {EditingDeckIndex + 1}");
            WndManager.Instance.GetWnd<CardListWnd>()?.RefreshDeckDisplay();
        }
        
        /// <summary>
        /// 检查某张卡牌是否存在于当前编辑的卡组中
        /// </summary>
        public bool IsCardInCurrentDeck(int cardID)
        {
            var deck = Decks[EditingDeckIndex];
            return deck.Cards.Exists(c => c.CardID == cardID);
        }
        
        /// <summary>
        /// 获取当前编辑的卡组中的卡牌总数
        /// </summary>
        public int GetCurrentDeckCardCount()
        {
            var deck = Decks[EditingDeckIndex];
            int totalCount = 0;
            foreach (var entry in deck.Cards)
            {
                totalCount += entry.Count;
            }
            return totalCount;
        }

        /// <summary>
        /// 获取指定卡组的卡牌总数
        /// </summary>
        public int GetDeckCardCount(int deckIndex)
        {
            if (deckIndex < 0 || deckIndex >= 3)
            {
                Debug.LogWarning("无效的卡组索引");
                return 0;
            }
            var deck = Decks[deckIndex];
            int totalCount = 0;
            foreach (var entry in deck.Cards)
            {
                totalCount += entry.Count;
            }
            return totalCount;
        }

        #endregion
    }
}