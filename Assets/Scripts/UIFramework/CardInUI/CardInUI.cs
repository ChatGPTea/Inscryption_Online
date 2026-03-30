using System.Collections;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Inscryption
{
    public class CardInUI : InscryptionController
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
        private Button ChoiceBtn;
        private RectTransform ChoiceBtnRect;

        //可保存，传递数据
        public CardInHandData mData;
        

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
            ChoiceBtn = transform.Find("ChoiceBtn").GetComponent<Button>();
            ChoiceBtnRect = transform.Find("ChoiceBtn").GetComponent<RectTransform>();
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
            
            ChoiceBtn.onClick.AddListener(() =>
            {
                WndManager.Instance.GetWnd<GameWnd>().CardInHandManager.PlayerDrawMainCard(mData.CardId);
                this.GetSystem<ICardSystem>().mPlayerCardIDList.Remove(mData.CardId);
                WndManager.Instance.DeleteWnd<ChoiceCardWnd>();
            });
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
            var _randomNum = Random.Range(1, 11);
            AudioKit.PlaySound("resources://Sound/Cards/card" + _randomNum);
            if (mCurrentState == CardInHandState.OnAnim) return;

            if (mCurrentState == CardInHandState.Idle)
            {
                mCurrentState = CardInHandState.Selected;
                OnSelected();
            }
            else if (mCurrentState == CardInHandState.Selected)
            {
                mCurrentState = CardInHandState.Idle;
                UnSelected();
            }
        }
        
        public void OnSelected()
        {
            if (mCurrentState == CardInHandState.OnAnim) return;
            //用于取消其他卡牌的被选中状态
            CancelOtherCardsSelection();

            //动画逻辑
            transform.DOKill();
            ChoiceBtn.gameObject.SetActive(true);
            ChoiceBtnRect.localPosition = new Vector3(mCurrentPosition, -130f, 0);
            ChoiceBtnRect.DOAnchorPos(new Vector2(ChoiceBtnRect.transform.localPosition.x, -230f), 0.3f).SetEase(Ease.OutBack);
        }
        
        private void CancelOtherCardsSelection()
        {
            for (int i = 0; i < transform.parent.parent.childCount; i++)
            {
                Transform sibling = transform.parent.parent.GetChild(i);

                if (sibling == transform.parent) continue;
                var cardComponent = sibling.GetComponentInChildren<CardInUI>();
                if (cardComponent != null && cardComponent.mCurrentState == CardInHandState.Selected)
                {
                    cardComponent.UnSelected();
                }
            }
        }

        public void UnSelected()
        {
            mCurrentState = CardInHandState.Idle;

            transform.SetSiblingIndex(mCurrentIndex);
            ChoiceBtnRect.DOAnchorPos(new Vector2(ChoiceBtnRect.transform.localPosition.x, -130), 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                ChoiceBtn.gameObject.SetActive(false);
            });
        }
    }
}