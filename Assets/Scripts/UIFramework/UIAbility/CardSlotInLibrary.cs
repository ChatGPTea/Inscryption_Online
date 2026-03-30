using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inscryption
{
    public class CardSlotInLibrary : InscryptionController
    {
        public Image mCounter;
        public TMP_Text Name;
        private CountInfo mCountInfo;
        private int mCurrentCardID;
        public Button mButton;
        

        private void Awake()
        {
            mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;
        }

        public void Init(CardInfo cardInfo)
        {
            Name.text = cardInfo.Name;
            mCurrentCardID = cardInfo.CardID;
            //条件判断
            if (mCountInfo.FindPixelSpriteWithCostAndCostType(cardInfo.Cost, cardInfo.CostType) == null)
            {
                mCounter.gameObject.SetActive(false);
            }
            else
            {
                mCounter.gameObject.SetActive(true);
                mCounter.sprite = mCountInfo.FindPixelSpriteWithCostAndCostType(cardInfo.Cost, cardInfo.CostType);
            }
            
            mButton.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            WndManager.Instance.GetWnd<CardListWnd>().CloseCardInfo();
            DeckSystem.Instance.RemoveCardFromDeck(mCurrentCardID);
        }
        
        
    }
}