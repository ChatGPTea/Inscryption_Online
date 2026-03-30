using DG.Tweening;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class Slot : InscryptionController , IOnEvent<TestMono.EnemySlotSelectEvent> , IOnEvent<TestMono.SummonCardInSlotEvent>, IOnEvent<GameNet.EnemySlotSelectEvent> 
    {
        [Tooltip("最大Y轴偏移距离（向上/下展开）")] public float maxVerticalOffset = 100f;
        [Tooltip("最大旋转角度（左右展开）")] public float maxRotationAngle = 30f;

        public int TotalSlots;
        public int slotIndex;
        public Transform slotVisual;
        private GameObject mCardInSlot;
        public void Init()
        {
            //本地玩家获取当前房间的格子数量
            TotalSlots = this.SendQuery(new LocalSlotsCountQuery());

            #region 获取索引

            slotVisual = transform.Find("SlotVisual");
            if (slotVisual == null)
            {
                Debug.LogError($"Slot {transform.name} 中未找到名为 SlotVisual 的子物体！", this);
                return;
            }

            var parent = transform.parent;
            if (parent == null)
            {
                Debug.LogError("Slot 的父物体不存在！", this);
                return;
            }

            slotIndex = 0;
            foreach (Transform child in parent)
            {
                if (child == transform)
                    break;
                slotIndex++;
            }

            #endregion

            ApplyFanOffset();
            
            this.RegisterEvent<TestMono.EnemySlotSelectEvent>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<TestMono.SummonCardInSlotEvent>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.RegisterEvent<GameNet.EnemySlotSelectEvent>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        ///     根据当前 Slot 的索引，计算并应用 Y 轴与 Rotation 的偏移，形成扇形效果
        /// </summary>
        private void ApplyFanOffset()
        {
            if (slotVisual == null) return;

            var t = Mathf.Clamp01((float)slotIndex / Mathf.Max(1, TotalSlots - 1));

            var yOffset = Mathf.Sin(t * Mathf.PI) * maxVerticalOffset;
            var angleOffset = Mathf.Lerp(-maxRotationAngle, maxRotationAngle, t);

            // 应用到 SlotVisual
            slotVisual.localPosition = new Vector3(0, yOffset, 0);
            slotVisual.localRotation = Quaternion.Euler(0, 0, angleOffset);
        }

        #region 献祭

        //献祭相关
        public bool isMySlot = false;

        //玩家点击后向目前符合条件的手牌发送事件
        public struct SlotClickEvent
        {
            public int slotIndex;
            public Transform slotVisual;
        }

        public void ClickSlot()
        {
            if (!isMySlot && WndManager.Instance.GetWnd<GameWnd>().isReady)
            {
                WndManager.Instance.GetWnd<GameWnd>().TryPlaceCardAtEnemySlot();
            }
            if (!isMySlot) return;
            if (this.GetSystem<ITurnSystem>().State.Value == TurnState.AfterDrawCard ||
                this.GetSystem<ITurnSystem>().State.Value == TurnState.PlayerFirstTurnBegin)
            {
                if (WndManager.Instance.GetWnd<GameWnd>().isReady)
                {
                    TypeEventSystem.Global.Send(new SlotClickEvent()
                    {
                        slotIndex = slotIndex,
                        slotVisual = slotVisual
                    });
                }
                else
                {
                    if (WndManager.Instance.GetWnd<GameWnd>().mCost == -1000) return;
                    var cardName = this.SendQuery(
                        new FindCardInfoWithIDQuery(WndManager.Instance.GetWnd<GameWnd>().CurrentCardData.CardId)).Name;
                    var cost = WndManager.Instance.GetWnd<GameWnd>().mCost;
                    WndManager.Instance.GetWnd<GameWnd>().TryPlaceWhenNotReady(cardName,cost);
                }
            }
        }
        
        //敌人同步生成造物：从手牌召唤
        public void OnEvent(TestMono.EnemySlotSelectEvent e)
        {
            if (isMySlot) return;
            if (e.slotIndex == slotIndex)
            {
                this.SendCommand(new EnemySummonFormHandCommand(this));
            }
        }

        public void OnEvent(GameNet.EnemySlotSelectEvent e)
        {
            if (isMySlot) return;
            if (e.slotIndex == slotIndex)
            {
                this.SendCommand(new EnemySummonFormHandCommand(this));
            }
        }
        
        public void OnEvent(TestMono.SummonCardInSlotEvent e)
        {
            if (isMySlot != e.isMyCard) return;
            if (slotIndex != e.slotIndex) return;
            mCardInSlot = Resources.Load<GameObject>("Prefabs/Card_InSlot");
            var CardInSlot = Instantiate(mCardInSlot, slotVisual);
            var cardInSlot = CardInSlot.GetComponent<CardInSlot>();
            cardInSlot.Init(e.cardID, e.isMyCard);
            this.SendCommand(new AddCardInSlotCommand(CardInSlot, slotIndex, isMySlot));
            cardInSlot.isMycard = isMySlot;
            cardInSlot.mBG.color = new Color32(255, 255, 255, 0);
            cardInSlot.mFace.color = new Color32(255, 255, 255, 0);
            var sequence = DOTween.Sequence();
            sequence.Append(cardInSlot.mBG.DOColor(Color.white, 0.15f))
                    .Join(cardInSlot.mFace.DOColor(Color.white, 0.15f));
        }

        public void SummonCardInSlot(int cardID, bool isMycard)
        {
            mCardInSlot = Resources.Load<GameObject>("Prefabs/Card_InSlot");
            var CardInSlot = Instantiate(mCardInSlot, slotVisual);
            CardInSlot.transform.SetAsLastSibling();
            var cardInSlot = CardInSlot.GetComponent<CardInSlot>();
            cardInSlot.Init(cardID, isMycard);
            this.SendCommand(new AddCardInSlotCommand(CardInSlot, slotIndex, isMySlot));
            cardInSlot.isMycard = isMySlot;
            cardInSlot.mBG.color = new Color32(255, 255, 255, 0);
            cardInSlot.mFace.color = new Color32(255, 255, 255, 0);
            var sequence = DOTween.Sequence();
            sequence.Append(cardInSlot.mBG.DOColor(Color.white, 0.15f))
                .Join(cardInSlot.mFace.DOColor(Color.white, 0.15f));
        }
        
        public void SummonCardInSlotLater(int cardID,bool isMycard)
        {
            this.GetSystem<ITimeSystem>().AddDelayTask(0.5f, () =>
            {
                mCardInSlot = Resources.Load<GameObject>("Prefabs/Card_InSlot");
                var CardInSlot = Instantiate(mCardInSlot, slotVisual);
                var cardInSlot = CardInSlot.GetComponent<CardInSlot>();
                cardInSlot.Init(cardID,isMycard);
                this.SendCommand(new AddCardInSlotCommand(CardInSlot, slotIndex, isMySlot));
                cardInSlot.isMycard = isMySlot;
                cardInSlot.mBG.color = new Color32(255, 255, 255, 0);
                cardInSlot.mFace.color = new Color32(255, 255, 255, 0);
                var sequence = DOTween.Sequence();
                sequence.Append(cardInSlot.mBG.DOColor(Color.white, 0.15f))
                    .Join(cardInSlot.mFace.DOColor(Color.white, 0.15f));
            });
        }

        #endregion



    }
}