using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inscryption
{
    public class Detail : InscryptionController
    {
        public TMP_Text Name;
        public TMP_Text Description;
        public TMP_Text ATK;
        public TMP_Text HP;

        public Image mCounter;
        private CountInfo mCountInfo;
        private CardToolTip_SO mCardToolTip_SO;

        private int mCurrentCardID;

        // 翻页相关
        public Button mLeftPageButton;
        public Button mRightPageButton;
        private int currentPageIndex = 0;
        private int totalPages = 1;

        private void Awake()
        {
            mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;
            mCardToolTip_SO = Resources.Load("SO/Ability/印记信息") as CardToolTip_SO;

            // 绑定翻页事件
            if (mLeftPageButton != null)
                mLeftPageButton.onClick.AddListener(OnLeftPageClick);

            if (mRightPageButton != null)
                mRightPageButton.onClick.AddListener(OnRightPageClick);
        }

        public void Init(CardInfo cardInfo)
        {
            Name.text = cardInfo.Name;
            mCurrentCardID = cardInfo.CardID;
            ATK.text = cardInfo.Attack.ToString();
            HP.text = cardInfo.Health.ToString();

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

            // 设置 Description 使用 TMP 分页
            SetupDescriptionPagination(cardInfo);

            // 初始化页码
            currentPageIndex = 0;
            totalPages = Mathf.Max(1, Description.textInfo.pageCount);

            // 更新按钮状态
            UpdatePageButtons();

            // 根据页数显示/隐藏翻页按钮
            UpdatePageButtonsVisibility();
            
            ShowPage(currentPageIndex);
        }

        /// <summary>
        /// 设置 Description 的分页功能
        /// </summary>
        private void SetupDescriptionPagination(CardInfo cardInfo)
        {
            // 构建完整的描述文本
            string fullDescription = "";
            foreach (var ability in cardInfo.Abilities)
            {
                var tip = mCardToolTip_SO.GetCardAbilityTip(ability);
                if (tip != null)
                {
                    fullDescription += tip.abliltyName + ":" + tip.description + "\n";
                }
            }

            // 如果没有内容，显示提示
            if (string.IsNullOrEmpty(fullDescription))
            {
                fullDescription = "";
            }

            // 设置文本
            Description.text = fullDescription;

            // 确保 TMP 已计算布局
            // 使用 Canvas.ForceUpdateCanvases() 确保布局计算完成
            Canvas.ForceUpdateCanvases();
            Description.ForceMeshUpdate();

            // 设置 overflow 为 page
            Description.overflowMode = TextOverflowModes.Page;

            // 确保有足够多的页面
            totalPages = Mathf.Max(1, Description.textInfo.pageCount);
        }

        /// <summary>
        /// 显示指定页
        /// </summary>
        private void ShowPage(int pageIndex)
        {
            if (totalPages <= 1) return;

            // 确保索引在有效范围内
            pageIndex = Mathf.Clamp(pageIndex, 0, totalPages - 1);
            currentPageIndex = pageIndex;

            // 使用 TMP 的 pageToDisplay 属性跳转到指定页
            Description.pageToDisplay = currentPageIndex + 1; // TMP 页码从 1 开始

            // 更新按钮状态
            UpdatePageButtons();
        }

        /// <summary>
        /// 更新翻页按钮的可见性
        /// </summary>
        private void UpdatePageButtonsVisibility()
        {
            bool shouldShowButtons = totalPages > 1;
            mLeftPageButton.gameObject.SetActive(shouldShowButtons);
            mRightPageButton.gameObject.SetActive(shouldShowButtons);
        }

        /// <summary>
        /// 更新翻页按钮的可用状态
        /// </summary>
        private void UpdatePageButtons()
        {
            if (mLeftPageButton != null)
            {
                mLeftPageButton.interactable = currentPageIndex > 0;
            }

            if (mRightPageButton != null)
            {
                mRightPageButton.interactable = currentPageIndex < totalPages - 1;
            }
        }

        /// <summary>
        /// 左翻页按钮点击
        /// </summary>
        private void OnLeftPageClick()
        {
            if (currentPageIndex > 0)
            {
                ShowPage(currentPageIndex - 1);
            }
        }

        /// <summary>
        /// 右翻页按钮点击
        /// </summary>
        private void OnRightPageClick()
        {
            if (currentPageIndex < totalPages - 1)
            {
                ShowPage(currentPageIndex + 1);
            }
        }
    }
}

