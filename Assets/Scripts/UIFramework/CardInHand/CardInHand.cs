using System.Collections;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Inscryption
{
    public class CardInHand : InscryptionController, IOnEvent<Slot.SlotClickEvent>
    {
        //卡牌状态
        public CardInHandState mCurrentState;
        public int CardIndex;

        //能力组件
        private Transform mAbility_1;
        private Transform mAbility_2;
        private Transform mAbility_3;


        //获取UI组件
        private TMP_Text mAttackText;
        private TMP_Text mHPText;
        private TMP_Text mNameText;
        private Image Ability_1_1;
        private Image Ability_2_1;
        private Image Ability_2_2;
        private Image Ability_3_1;
        private Image mDecal;
        private Image mFace;
        private Image mFace_Add;
        public Image mBG;
        public Image Back;
        private Image mCounter;
        private CountInfo mCountInfo;
        public CardInfo mCurrentCardInfo;
        public int mCurrentCardID;
        private int mCurrentIndex;
        private float mCurrentPosition;


        //可保存，传递数据
        public CardInHandData mData;

        //Bool标志位
        public bool isMycard = true;

        private void Awake()
        {
            //获取UI组件
            mBG = transform.Find("BG").GetComponent<Image>();
            Back = transform.Find("Back").GetComponent<Image>();
            mDecal = transform.Find("Decal").GetComponent<Image>();
            mFace = transform.Find("Face").GetComponent<Image>();
            mFace_Add = transform.Find("Face_Add").GetComponent<Image>();
            mCounter = transform.Find("Counter").GetComponent<Image>();
            mNameText = transform.Find("Name").GetComponent<TMP_Text>();
            mAttackText = transform.Find("Attack").GetComponent<TMP_Text>();
            mHPText = transform.Find("HP").GetComponent<TMP_Text>();
            //获取能力组件
            mAbility_1 = transform.Find("Ability_1").gameObject.transform;
            mAbility_2 = transform.Find("Ability_2").gameObject.transform;
            mAbility_3 = transform.Find("Ability_3").gameObject.transform;
            Ability_1_1 = transform.Find("Ability_1/Ability_1_1").GetComponent<Image>();
            Ability_2_1 = transform.Find("Ability_2/Ability_2_1").GetComponent<Image>();
            Ability_2_2 = transform.Find("Ability_2/Ability_2_2").GetComponent<Image>();
            Ability_3_1 = transform.Find("Ability_3/Add/Ability_3_1").GetComponent<Image>();
            //读取SO文件
            mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;

            //设置初始状态
            mCurrentState = CardInHandState.OnAnim;

            //绑定事件
            this.RegisterEvent<Slot.SlotClickEvent>()
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        /// <summary>
        ///     初始化手牌
        /// </summary>
        public void Init(int cardId)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));

            mData = new CardInHandData
            {
                CardId = mCurrentCardInfo.CardID,
                Health = mCurrentCardInfo.Health,
                Attack = mCurrentCardInfo.Attack,
                Cost = mCurrentCardInfo.Cost,
                Abilities = mCurrentCardInfo.Abilities,
                Abilities_Sprite = mCurrentCardInfo.Abilities_Sprite
            };

            ReDraw();
        }

        public void Init(int cardId, CardInHandData data)
        {
            mCurrentCardID = cardId;
            mCurrentCardInfo = this.SendQuery(new FindCardInfoWithIDQuery(cardId));

            mData = data;

            ReDraw();
        }

        public void ReDraw()
        {
            mBG.sprite = mCurrentCardInfo.BackGround == null ? mBG.sprite : mCurrentCardInfo.BackGround;
            mDecal.sprite = mCurrentCardInfo.Decal == null ? mDecal.sprite : mCurrentCardInfo.Decal;
            mFace.sprite = mCurrentCardInfo.Face == null ? mFace.sprite : mCurrentCardInfo.Face;
            mFace_Add.sprite = mCurrentCardInfo.Face_Add == null ? mFace_Add.sprite : mCurrentCardInfo.Face_Add;
            mNameText.text = mCurrentCardInfo.Name == null ? mNameText.text : mCurrentCardInfo.Name;
            mAttackText.text = mData.Attack.ToString();
            mHPText.text = mData.Health.ToString();

            //条件判断
            if (mCountInfo.FindSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType) == null)
            {
                mCounter.gameObject.SetActive(false);
            }
            else
            {
                mCounter.gameObject.SetActive(true);
                mCounter.sprite = mCountInfo.FindSpriteWithCostAndCostType(mData.Cost, mCurrentCardInfo.CostType);
            }

            if (mData.Abilities.Count == 0)
            {
                mAbility_1.gameObject.SetActive(false);
                mAbility_2.gameObject.SetActive(false);
                mAbility_3.gameObject.SetActive(false);
            }
            else if (mData.Abilities.Count == 1)
            {
                mAbility_1.gameObject.SetActive(true);
                mAbility_2.gameObject.SetActive(false);
                mAbility_3.gameObject.SetActive(false);
                Ability_1_1.sprite = mData.Abilities_Sprite[0];
            }
            else if (mData.Abilities.Count == 2)
            {
                mAbility_1.gameObject.SetActive(false);
                mAbility_2.gameObject.SetActive(true);
                mAbility_3.gameObject.SetActive(false);
                Ability_2_1.sprite = mData.Abilities_Sprite[0];
                Ability_2_2.sprite = mData.Abilities_Sprite[1];
            }
            else if (mData.Abilities.Count == 3)
            {
                //TODO
            }
        }

        /// <summary>
        ///     鼠标点击事件
        /// </summary>
        public void Select()
        {
            if (!isMycard) return;
            if (WndManager.Instance.GetWnd<GameWnd>().isReady && mData.Cost != 0) return;
            if (mCurrentState == CardInHandState.OnAnim) return;

            if (mCurrentState == CardInHandState.Idle)
            {
                mCurrentState = CardInHandState.Selected;
                OnSelected();
                GameNet.Instance.SyncSelectCard(CardIndex);
            }
            else if (mCurrentState == CardInHandState.Selected)
            {
                mCurrentState = CardInHandState.Idle;
                UnSelected();
                GameNet.Instance.SyncSelectCard(CardIndex);
            }
        }

        public void OnSelected()
        {
            var _randomNum = Random.Range(1, 11);
            AudioKit.PlaySound("resources://Sound/Cards/card" + _randomNum);
            if (mCurrentState == CardInHandState.OnAnim) return;
            //用于取消其他卡牌的被选中状态
            CancelOtherCardsSelection();

            //动画逻辑
            transform.DOKill();
            mCurrentPosition = transform.position.y;
            mCurrentIndex = transform.GetSiblingIndex();
            transform.SetAsLastSibling();

            if (isMycard)
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(
                    transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f)
                        .SetEase(Ease.OutBack));
                sequence.Join(transform.DOMoveY(mCurrentPosition + 1f, 0.2f).SetEase(Ease.OutBack));
                sequence.Play();

                //显示卡牌详情
                if (isMycard) WndManager.Instance.GetWnd<GameWnd>().ReDrawCardInfoTip(gameObject,false);
            }
            else
            {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(
                    transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.2f)
                        .SetEase(Ease.OutBack));
                sequence.Join(transform.DOMoveY(mCurrentPosition - 1f, 0.2f).SetEase(Ease.OutBack));
                sequence.Play();
            }
        }

        private void CancelOtherCardsSelection()
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform sibling = transform.parent.GetChild(i);

                if (sibling == transform) continue;
                var cardComponent = sibling.GetComponent<CardInHand>();
                if (cardComponent != null && cardComponent.mCurrentState == CardInHandState.Selected)
                {
                    cardComponent.UnSelected();
                }
            }
        }

        public void UnSelected()
        {
            mCurrentState = CardInHandState.Idle;
            //动画逻辑
            transform.DOKill();
            var sequence = DOTween.Sequence();
            sequence.Append(
                transform.DOScale(new Vector3(0.85f, 0.85f, 0.85f), 0.2f)
                    .SetEase(Ease.OutSine));
            sequence.Join(transform.DOMoveY(mCurrentPosition, 0.2f).SetEase(Ease.OutBack));

            // 播放序列
            sequence.Play();

            transform.SetSiblingIndex(mCurrentIndex);

            //隐藏卡牌详情
            if (isMycard) WndManager.Instance.GetWnd<GameWnd>().HideCardInfoTip();
        }

        //献祭相关
        private GameObject mCardInSlot;

        //己方通过献祭生成卡牌
        public void OnEvent(Slot.SlotClickEvent e)
        {
            if (!isMycard) return;
            if (mCurrentState == CardInHandState.Selected)
            {
                mCurrentState = CardInHandState.OnAnim;
                GameNet.Instance.SyncPlaceCard(e.slotIndex);
                var sequence = DOTween.Sequence();
                sequence.Append(
                    transform.DOMove(e.slotVisual.position, 0.5f).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            // 动画完成后的回调 删除卡牌布局，删除System手牌
                            this.GetModel<ISlotsModel>().OnEnemyCardPlace(true,e.slotIndex);
                            WndManager.Instance.GetWnd<GameWnd>().HideCardInfoTip();
                            WndManager.Instance.GetWnd<GameWnd>().CardInHandManager.mPlayerCardLayout.Remove(transform);
                            this.SendCommand(new RemoveCardToPlayerCardListCommand(gameObject));

                            //生成CardinSlot
                            mCardInSlot = Resources.Load<GameObject>("Prefabs/Card_InSlot");
                            var CardInSlot = Instantiate(mCardInSlot, e.slotVisual);

                            CardInSlot.GetComponent<CardInSlot>().Init(mCurrentCardID, mData, true);
                            CardInSlot.GetComponent<CardInSlot>().isMycard = true;

                            this.SendCommand(new AddCardInSlotCommand(CardInSlot, e.slotIndex, true));
                            Destroy(gameObject);
                        }));
                sequence.Join(
                    transform.DORotateQuaternion(e.slotVisual.rotation, 0.45f)
                        .SetEase(Ease.OutQuad));
                sequence.Join(
                    transform.DOScale(new Vector3(0.66f, 0.66f, 0.66f), 0.45f)
                        .SetEase(Ease.OutQuad));
            }
        }

        //敌方通过献祭生成卡牌
        public void EnemySummonCardFormHand(Slot slot)
        {
            if (isMycard) return;
            if (mCurrentState == CardInHandState.Selected)
            {
                mCurrentState = CardInHandState.OnAnim;
                var sequence = DOTween.Sequence();
                sequence.Append(
                    transform.DOMove(slot.slotVisual.position, 0.5f).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            // 动画完成后的回调 删除卡牌布局，删除System手牌
                            this.GetModel<ISlotsModel>().OnEnemyCardPlace(false,slot.slotIndex);
                            WndManager.Instance.GetWnd<GameWnd>().CardInHandManager.mEnemyCardLayout.Remove(transform);
                            this.SendCommand(new RemoveCardToEnemyCardListCommand(gameObject));

                            //生成CardinSlot
                            mCardInSlot = Resources.Load<GameObject>("Prefabs/Card_InSlot");
                            var CardInSlot = Instantiate(mCardInSlot, slot.slotVisual);

                            CardInSlot.GetComponent<CardInSlot>().Init(mCurrentCardID, mData, false);
                            CardInSlot.GetComponent<CardInSlot>().isMycard = false;

                            this.SendCommand(new AddCardInSlotCommand(CardInSlot, slot.slotIndex, false));
                            Destroy(gameObject);
                        }));
                sequence.Join(
                    transform.DORotateQuaternion(slot.slotVisual.rotation, 0.45f)
                        .SetEase(Ease.OutQuad));
                sequence.Join(
                    transform.DOScale(new Vector3(0.66f, 0.66f, 0.66f), 0.45f)
                        .SetEase(Ease.OutQuad));
            }
        }
    }
}