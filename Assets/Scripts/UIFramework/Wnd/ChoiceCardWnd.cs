using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Inscryption
{
    public class ChoiceCardWnd : BaseWnd
    {
        private List<int> mCardList = new List<int>();
        private Transform content;
        private Button mLookingBtn;
        private Image BG;
        private List<GameObject> mCards = new List<GameObject>();
        public bool isLooking = false;
        public bool inAnimation = false;

        public override void Initial()
        {
            content = SelfTransform.Find("Content");
            mLookingBtn = SelfTransform.Find("LookingBtn").GetComponent<Button>();
            BG = SelfTransform.Find("BG").GetComponent<Image>();
            mLookingBtn.onClick.AddListener(OnLookingClick);
        }

        public void Init(List<int> cardList)
        {
            mCardList = cardList;
            mCards.Clear();
            // Fisher-Yates 洗牌算法
            for (int i = mCardList.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                // 交换当前元素和随机选择的元素
                (mCardList[i], mCardList[randomIndex]) = (mCardList[randomIndex], mCardList[i]);
            }

            // 清空Content下的现有子物体
            foreach (Transform child in content)
            {
                child.gameObject.DestroySelf();
            }

            GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Position");

            for (var i = 0; i < mCardList.Count; i++)
            {
                GameObject cardInstance = Object.Instantiate(cardPrefab, content);
                mCards.Add(cardInstance);
                var card = cardInstance.GetComponentInChildren<CardInUI>();
                var cardRect = cardInstance.transform.Find("Card_InUI").GetComponent<RectTransform>();
                cardRect.localPosition =
                    new Vector3(card.transform.localPosition.x, card.transform.localPosition.y + 80);
                cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutBack);
                card.Init(mCardList[i]);
            }
        }

        public void OnLookingClick()
        {
            if (inAnimation)
                return;
            if (isLooking)
            {
                isLooking = false;
                inAnimation = true;
                BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 0f);
                BG.DOFade(1f, 0.4f);
                foreach (var cardObj in mCards)
                {
                    var card = cardObj.GetComponentInChildren<CardInUI>();
                    var cardRect = cardObj.transform.Find("Card_InUI").GetComponent<RectTransform>();
                    cardObj.SetActive(true);
                    cardRect.localPosition =
                        new Vector3(card.transform.localPosition.x, card.transform.localPosition.y + 80);
                    cardRect.DOAnchorPos(Vector2.zero, 0.3f).SetEase(Ease.OutBack)
                        .OnComplete(() => { inAnimation = false; });
                }

            }
            else
            {
                isLooking = true;
                inAnimation = true;
                BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 1f);
                BG.DOFade(0f, 0.4f);
                foreach (var cardObj in mCards)
                {
                    var card = cardObj.GetComponentInChildren<CardInUI>();
                    var cardRect = cardObj.transform.Find("Card_InUI").GetComponent<RectTransform>();
                    cardRect.localPosition = Vector2.zero;
                    cardRect.DOAnchorPos(
                            new Vector3(card.transform.localPosition.x, card.transform.localPosition.y + 80), 0.3f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            cardObj.SetActive(false);
                            inAnimation = false;
                        });
                }
            }
        }
    }
}