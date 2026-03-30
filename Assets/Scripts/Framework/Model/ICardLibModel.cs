using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    public interface ICardLibModel : IModel
    {
        List<CardInfo> CardLib { get; set; }
    }

    public class CardListModel : AbstractModel, ICardLibModel
    {
        /// <summary>
        ///     加载So文件路径
        /// </summary>
        private readonly string[] cardFolders =
        {
            "SO/Cards/原版野兽",
            "SO/Cards/副卡组",
            "SO/Cards/测试用",
            "SO/Cards/原版野兽衍生物"
        };

        public List<CardInfo> CardLib { get; set; } = new();

        protected override void OnInit()
        {
            LoadAllCardInfos();
        }

        /// <summary>
        ///     加载所有指定文件夹中的 CardInfo ScriptableObject
        /// </summary>
        private void LoadAllCardInfos()
        {
            CardLib.Clear();

            foreach (var folderPath in cardFolders)
            {
                var cardsInFolder = Resources.LoadAll<CardInfo>(folderPath);
                if (cardsInFolder != null && cardsInFolder.Length > 0)
                {
                    CardLib.AddRange(cardsInFolder);
                }
            }

            Debug.Log($"总共加载了 {CardLib.Count} 张卡牌");
        }
    }
    
    // ========== 图鉴筛选与分页 ==========
    public interface ICardFilter
    {
        bool IsMatch(CardInfo card);
    }

    /// <summary>
    /// 费用数量筛选器
    /// </summary>
    public class CostAmountFilter : ICardFilter
    {
        public int MinCost { get; set; }
        public int MaxCost { get; set; }

        public CostAmountFilter(int min = 0, int max = int.MaxValue)
        {
            MinCost = min;
            MaxCost = max;
        }

        public bool IsMatch(CardInfo card)
        {
            return card.Cost >= MinCost && card.Cost <= MaxCost;
        }
    }

    /// <summary>
    /// 费用类型筛选器
    /// </summary>
    public class CostTypeFilter : ICardFilter
    {
        public CostType TargetCostType { get; set; }

        public CostTypeFilter(CostType costType)
        {
            TargetCostType = costType;
        }

        public bool IsMatch(CardInfo card)
        {
            return card.CostType == TargetCostType;
        }
    }
    
    public class DontGenerateInLibraryFilter : ICardFilter
    {
        public bool IsMatch(CardInfo card)
        {
            // 返回 true 表示保留，false 表示排除
            return !card.dontGenerateInLibrary;
        }
    }
    /// <summary>
    /// 组合筛选器（AND逻辑）
    /// </summary>
    public class CombinedFilter : ICardFilter
    {
        private List<ICardFilter> filters = new();

        public void AddFilter(ICardFilter filter)
        {
            filters.Add(filter);
        }

        public void ClearFilters()
        {
            filters.Clear();
            filters.Add(new DontGenerateInLibraryFilter());
        }

        public bool IsMatch(CardInfo card)
        {
            foreach (var f in filters)
            {
                if (!f.IsMatch(card))
                    return false;
            }
            return true;
        }
    }
}