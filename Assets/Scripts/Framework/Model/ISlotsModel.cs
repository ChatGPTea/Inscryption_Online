using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    public enum CardInSlotState
    {
        Idle,
        OnSacrifice,
        OnView,
        OnAnim,
    }

    public enum Buff
    {
        Ant,
        Stink,
    }

    public interface ISlotsModel : IModel
    {
        Dictionary<int, GameObject> PlayerCardInSlotDic { get; set; }
        Dictionary<int, GameObject> EnemyCardInSlotDic { get; set; }

        public void UpdateBuff();
        public void OnEnemyCardPlace(bool isMyCard, int slotID);
        public void OnEnemyTryAttack(bool isMyCard, int slotID);
        public void OnTurnBegin(bool isMyCard);
        public void OnTurnEnd(bool isMyCard);
        public int CurrentSlot { get; set; }
        void ReSetSlots(int slotCount);
    }

    public class SlotModel : AbstractModel, ISlotsModel, ICanGetModel
    {
        public Dictionary<int, GameObject> PlayerCardInSlotDic { get; set; }
        public Dictionary<int, GameObject> EnemyCardInSlotDic { get; set; }
        public int CurrentSlot { get; set; }
        public List<CardInSlot> EffectList  { get; set; } = new List<CardInSlot>();
        protected override void OnInit()
        {
            PlayerCardInSlotDic = new Dictionary<int, GameObject>();
            EnemyCardInSlotDic = new Dictionary<int, GameObject>();

            ReSetSlots(this.GetModel<IRoomModel>().SlotCount.Value);
        }

        //格子即将承受攻击
        public void OnEnemyTryAttack(bool isMyCard, int SlotID)
        {
            CurrentSlot = SlotID;
            var Dic = !isMyCard ? PlayerCardInSlotDic : EnemyCardInSlotDic;
            for (var i = 0; i < this.GetModel<IRoomModel>().SlotCount.Value; i++)
            {
                var obj = Dic[i];
                if (obj.GetComponent<CardInSlot>() != null)
                {
                    var card = obj.GetComponent<CardInSlot>();
                    if (i != SlotID && card.GetComponentInChildren<LooseTail断尾求生>() != null)
                        continue;
                    foreach (var effect in card.Effects)
                    {
                        effect.Init();
                        effect.BeforeTakeAttackExecute();
                    }
                }

            }

            CurrentSlot = -1;
        }

        //己方放置卡牌，触发对面印记
        public void OnEnemyCardPlace(bool isMyCard, int SlotID)
        {
            CurrentSlot = SlotID;
            var Dic = !isMyCard ? PlayerCardInSlotDic : EnemyCardInSlotDic;
            for (var i = 0; i < this.GetModel<IRoomModel>().SlotCount.Value; i++)
            {
                var obj = Dic[i];
                if (obj.GetComponent<CardInSlot>() != null)
                {
                    var card = obj.GetComponent<CardInSlot>();
                    foreach (var effect in card.Effects)
                    {
                        effect.Init();
                        effect.OnEnemyPlaceExecute();
                    }
                }
            }

            CurrentSlot = -1;
        }

        public void OnTurnBegin(bool isMyCard)
        {
            EffectList.Clear();
            var Dic = isMyCard ? PlayerCardInSlotDic : EnemyCardInSlotDic;
            for (var i = 0; i < this.GetModel<IRoomModel>().SlotCount.Value; i++)
            {

                var obj = Dic[i];
                if (obj.GetComponent<CardInSlot>() != null)
                {
                    EffectList.Add(obj.GetComponent<CardInSlot>());
                }
            }

            for (var i = 0; i < EffectList.Count; i++)
            {
                var card = EffectList[i];
                foreach (var effect in card.Effects)
                {
                    effect.Init();
                    effect.OnPlayerTurnBeginExecute();
                }
            }
        }

        public void OnTurnEnd(bool isMyCard)
        {
            EffectList.Clear();
            var Dic = isMyCard ? PlayerCardInSlotDic : EnemyCardInSlotDic;
            for (var i = 0; i < this.GetModel<IRoomModel>().SlotCount.Value; i++)
            {

                var obj = Dic[i];
                if (obj.GetComponent<CardInSlot>() != null)
                {
                    EffectList.Add(obj.GetComponent<CardInSlot>());
                }
            }

            for (var i = 0; i < EffectList.Count; i++)
            {
                var card = EffectList[i];
                foreach (var effect in card.Effects)
                {
                    effect.Init();
                    effect.OnPlayerTurnEndExecute();
                }
            }
        }


        public void UpdateBuff()
        {
            var AntPlayerCounter = 0;
            var AntEnemyCounter = 0;
            List<CardInSlot> AntCardList = new List<CardInSlot>();
            List<CardInSlot> StinkCardList = new List<CardInSlot>();
            List<CardInSlot> cards = new List<CardInSlot>();
            //上Buff
            for (var i = 0; i < this.GetModel<IRoomModel>().SlotCount.Value; i++)
            {
                var MyObj = PlayerCardInSlotDic[i];
                var EnemyObj = EnemyCardInSlotDic[i];
                if (MyObj.GetComponent<CardInSlot>() != null)
                {
                    var card = MyObj.GetComponent<CardInSlot>();
                    cards.Add(card);
                    card.Buffs.Clear();
                    if (MyObj.GetComponent<Ants种族蚂蚁>() != null)
                    {
                        AntPlayerCounter++;
                        AntCardList.Add(card);
                    }

                    if (EnemyObj.GetComponent<CardInSlot>() != null && EnemyObj.GetComponent<Stink臭臭>() != null)
                    {
                        StinkCardList.Add(card);
                    }
                }

                if (EnemyObj.GetComponent<CardInSlot>() != null)
                {
                    var card = EnemyObj.GetComponent<CardInSlot>();
                    cards.Add(card);
                    card.Buffs.Clear();
                    if (EnemyObj.GetComponent<Ants种族蚂蚁>() != null)
                    {
                        AntEnemyCounter++;
                        AntCardList.Add(card);
                    }
                    
                    if (MyObj.GetComponent<CardInSlot>() != null && MyObj.GetComponent<Stink臭臭>() != null)
                    {
                        StinkCardList.Add(card);
                    }
                }
            }
            

            //结算buff
            foreach (var card in AntCardList)
            {
                card.Buffs.Add(Buff.Ant, card.isMycard ? AntPlayerCounter : AntEnemyCounter);
            } 
            foreach (var card in StinkCardList)
            {
                card.Buffs.Add(Buff.Stink, card.isMycard ? AntPlayerCounter : AntEnemyCounter);
            }

            foreach (var card in cards)
            {
                // 先算蚂蚁buff
                if (card.Buffs.TryGetValue(Buff.Ant, out int value))
                {
                    card.mData.Attack = card.OriginATK + value - 1;
                }
                else
                {
                    // 没有蚂蚁buff就恢复基础攻击
                    card.mData.Attack = card.OriginATK;
                }

                // 结算臭臭buff：只要有就减1攻
                if (card.Buffs.ContainsKey(Buff.Stink))
                {
                    card.mData.Attack -= 1;
                }
                
                if (card.mData.Attack < 0)
                    card.mData.Attack = 0;
                card.ReDraw();
            }
        }

        public void ReSetSlots(int index)
        {
            PlayerCardInSlotDic.Clear();
            EnemyCardInSlotDic.Clear();
            for (int i = 0; i < index; i++)
            {
                PlayerCardInSlotDic.Add(i, null);
                EnemyCardInSlotDic.Add(i, null);
            }
        }
    }
}