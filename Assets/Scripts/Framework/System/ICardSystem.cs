using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    /// <summary>
    ///     数据层枚举
    /// </summary>
    public enum CostType
    {
        sacrifice,
        bone,
        None
    }

    public enum CardType
    {
        beast,
        bones
    }

    public enum Race
    {
        Ant,
    }

    public enum Abilities
    {
        //原版野兽
        None,
        TouchOfDeath死神之触,
        Ants种族蚂蚁,
        GenerateAnts蚁后,
        DamBuilder筑坝师,
        Airborne空袭,
        BeesWithin内心之蜂,
        Guardian守护者,
        Reach高跳,
        GenerateWolf监禁,
        ManyLives生生不息,
        ChimeKeeper鸣钟人,
        Sprinter冲刺能手,
        Fledgling幼雏,
        Fecundity丰产之巢,
        WorthySacrifice优质祭品,
        Waterborne水袭,
        Hoarder囤积狂,
        BifurcatedStrike兵分两路,
        TrifurcatedStrike兵分三路,
        Burrower钻地龙,
        Unkillable不死之虫,
        SharpQuills尖刺铠甲,
        LooseTail断尾求生,
        Stink臭臭,
        RabbitHole兔子洞,
        Leader领袖力量,
        CorpseEater食尸鬼,
        BoneKing骨皇,
        MirrorATK镜像,
        HandATK,
        DistanceATK,
        BoneDigger,
        DoubleStrike,
        BroodParasite,
        Armored,
        OneHalfBones
    }

    /// <summary>
    ///     表现层枚举
    /// </summary>
    public enum CardInHandState
    {
        Idle,
        Selected,
        OnAnim
    }
    
    public struct CardInHandData
    {
        public int CardId;
        public int Health;
        public int Attack;
        public int Cost;
        public List<Abilities> Abilities;
        public List<Sprite> Abilities_Sprite;
    }
    
    [System.Serializable]
    public class AbilityTip
    {
        public Abilities abilities;
        public string abliltyName;
        public string description;
    }

    public interface ICardSystem : ISystem
    {
        List<GameObject> mPlayerCardInHands { get; set; }
        List<GameObject> mEnemyCardInHands { get; set; }
        List<int> mPlayerCardIDList { get; set; }
        List<int> mEnemyCardIDList { get; set; }
        int mPlayerCardCountMax { get; set; }
        BindableProperty<int> mPlayerCardCount { get; set; }
        int mPlayerSecondCardID { get; set; }
        
        void ReDrawCardCount();
        void SetPlayerCard();
    }

    public class CardSystem : AbstractSystem, ICardSystem
    {
        //当前手牌
        public List<GameObject> mPlayerCardInHands { get; set; }

        public List<GameObject> mEnemyCardInHands { get; set; }

        //牌库
        public List<int> mPlayerCardIDList { get; set; }
        public List<int> mEnemyCardIDList { get; set; }
        public int mPlayerCardCountMax { get; set; }
        public BindableProperty<int> mPlayerCardCount { get; set; }
        public int mPlayerSecondCardID { get; set; }
        

        protected override void OnInit()
        {
            mPlayerCardInHands = new List<GameObject>();
            mEnemyCardInHands = new List<GameObject>();
            mPlayerCardIDList = new List<int>();
            mEnemyCardIDList = new List<int>();
            mPlayerCardCount = new BindableProperty<int>(-1);

            //初始化玩家牌库
            for (var i = 0; i < 20; i++) mPlayerCardIDList.Add(1001);
            mPlayerSecondCardID = 1;
            mPlayerCardCountMax = mPlayerCardIDList.Count;
            mPlayerCardCount.Value = mPlayerCardCountMax;

            //注册变量事件
            mPlayerCardCount.Register(newValue =>
            {
                ReDrawCardCount();
            });


        }

        public void ReDrawCardCount()
        {
            WndManager.Instance?.GetWnd<GameWnd>().ReDrawCardCount(mPlayerCardCount.Value,mPlayerCardCountMax);
        }

        public void SetPlayerCard()
        {
            mPlayerCardInHands = new List<GameObject>();
            mEnemyCardInHands = new List<GameObject>();
            mPlayerCardIDList = new List<int>();
            mEnemyCardIDList = new List<int>();
            mPlayerCardCount = new BindableProperty<int>(-1);
            mPlayerCardIDList.Clear();
            var cards = DeckSystem.Instance.Decks[DeckSystem.Instance.EditingDeckIndex].Cards;
            foreach (var card in cards)
            {
                mPlayerCardIDList.Add(card.CardID);
            }
            // Fisher-Yates 洗牌算法
            for (int i = mPlayerCardIDList.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                (mPlayerCardIDList[i], mPlayerCardIDList[randomIndex]) = (mPlayerCardIDList[randomIndex], mPlayerCardIDList[i]);
            }
            mPlayerCardCountMax = mPlayerCardIDList.Count;
            mPlayerCardCount.Value = mPlayerCardCountMax;
        }
        
    }
}