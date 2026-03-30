using System.Collections.Generic;
using System.Linq;
using Inscryption;
using QFramework;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using EnjoyGameClub.TextLifeFramework.Core;
using TMPro;
using Transform = UnityEngine.Transform;

public class CardListWnd : BaseWnd, ICanGetModel, ICanSendQuery
{
    private Button mDeleteBtn;

    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;

    private Button mNextPageBtn;
    private Button mPreviousBtn;
    private TMP_Text mDebugText;
    private TMP_Text mCardCountText;
    private TextLife mTextLife;
    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    private Button mSaveBtn;

    ///图鉴组件
    private Toggle[] mToggles;

    private CardInLibrary[] mCards;

    /// <summary>
    ///  图鉴
    /// </summary>
    private ICardLibModel cardLibModel;

    // 筛选器
    public CombinedFilter Filter { get; private set; } = new CombinedFilter();

    // 分页
    public int CurrentPage { get; private set; } = 1;
    public int PageSize { get; private set; } = 10;

    public int TotalCards => FilteredCards.Count;
    public int TotalPages => Mathf.CeilToInt((float)TotalCards / PageSize);

    public List<CardInfo> FilteredCards
    {
        get { return cardLibModel.CardLib.Where(card => Filter.IsMatch(card)).ToList(); }
    }

    public List<CardInfo> CurrentPageCards
    {
        get
        {
            int startIndex = (CurrentPage - 1) * PageSize;
            int endIndex = Mathf.Min(startIndex + PageSize, TotalCards);
            if (startIndex >= TotalCards) return new List<CardInfo>();
            return FilteredCards.GetRange(startIndex, endIndex - startIndex);
        }
    }

    //卡组
    public Transform Content;
    public GameObject CardSlotInLibrary;
    public Detail mDetail;

    //卡牌详情
    private GridLayoutGroup mGridLayoutGroup;
    public Vector2 mOpenSpacing = new Vector2(7f, 65f);
    public Vector2 mCloseSpacing = new Vector2(80f, 65);

    public override void Initial()
    {
        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mNextPageBtn = SelfTransform.Find("NextPageBtn").GetComponent<Button>();
        mPreviousBtn = SelfTransform.Find("PreviousPageBtn").GetComponent<Button>();
        mDeleteBtn = SelfTransform.Find("DeleteBtn").GetComponent<Button>();
        mSaveBtn = SelfTransform.Find("SaveBtn").GetComponent<Button>();
        mToggles = SelfTransform.Find("CostSetting").GetComponentsInChildren<Toggle>();
        mCards = SelfTransform.Find("CardLibBG/CardLib").GetComponentsInChildren<CardInLibrary>();
        mDebugText = SelfTransform.Find("Tip_2").GetComponent<TextMeshProUGUI>();
        mTextLife = SelfTransform.Find("Tip_2").GetComponent<TextLife>();
        mCardCountText = SelfTransform.Find("ListNameTip_2").GetComponent<TextMeshProUGUI>();
        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mNextPageBtn.gameObject.AddComponent<CommandButtonMono>();
        mNextPageBtn.onClick.AddListener(NextPage);
        mPreviousBtn.gameObject.AddComponent<CommandButtonMono>();
        mPreviousBtn.onClick.AddListener(PreviousPage);
        mDeleteBtn.gameObject.AddComponent<CommandButtonMono>();
        mSaveBtn.gameObject.AddComponent<CommandButtonMono>();

        Content = SelfTransform.Find("Scroll View/Viewport/Content");
        CardSlotInLibrary = Resources.Load<GameObject>("Prefabs/CardSlotInLibrary");
        mDetail = SelfTransform.Find("CardDetail").GetComponent<Detail>();
        mGridLayoutGroup = SelfTransform.Find("CardLibBG/CardLib").GetComponent<GridLayoutGroup>();
        cardLibModel = this.GetModel<ICardLibModel>();
        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToLobby);
        mSaveBtn.onClick.AddListener(ClickSaveBtn);
        mDeleteBtn.onClick.AddListener(ClickDeleteBtn);
        foreach (var toggle in mToggles)
        {
            toggle.isOn = false;
        }

        //  绑定 Toggle 事件
        foreach (var toggle in mToggles)
        {
            toggle.isOn = false;
        }

        for (int i = 0; i < mToggles.Length; i++)
        {
            int index = i; // 捕获当前索引，避免闭包问题
            mToggles[i].onValueChanged.AddListener((isOn) => { OnToggleClicked(index, isOn); });
        }
        if(DeckSystem.Instance.GetCurrentDeckCardCount() != 15)
            DebugText("请添加手牌");
        Filter.ClearFilters();
        ApplyFilters();
        RefreshDeckDisplay();
        CloseCardInfoWwithNoAnimation();
    }

    private void ClickDeleteBtn()
    {
        DeckSystem.Instance.ClearCurrentDeck();
        CloseCardInfo();
        DebugText("清空牌组成功");

    }

    private void ClickSaveBtn()
    {
        if (DeckSystem.Instance.GetCurrentDeckCardCount() == 15)
        {
            DeckSystem.Instance.SaveDeck(DeckSystem.Instance.EditingDeckIndex,
                DeckSystem.Instance.Decks[DeckSystem.Instance.EditingDeckIndex]);
        }
        else if(DeckSystem.Instance.GetCurrentDeckCardCount() < 15)
        {
            DebugText("所选手牌不足  " + DeckSystem.Instance.GetCurrentDeckCardCount() + " / 15");
        }
        else if (DeckSystem.Instance.GetCurrentDeckCardCount() > 15)
        {
            DebugText("所选手牌过多  " + DeckSystem.Instance.GetCurrentDeckCardCount() + " / 15");
        }
        CloseCardInfo();
    }

    private void DebugText(string text)
    {
        mDebugText.text = "C:/Cards/Error: "+ text;
        mTextLife.ResetAnimation();
    }

    private void OnToggleClicked(int index, bool isOn)
    {
        if (!isOn)
        {
            // 如果取消选中，不做筛选
            Filter.ClearFilters();
            ApplyFilters();
            return;
        }

        // 取消其他 Toggle 的选中状态
        for (int i = 0; i < mToggles.Length; i++)
        {
            if (i != index)
            {
                mToggles[i].isOn = false;
            }
        }

        // 根据 index 确定费用范围
        int minCost, maxCost;
        if (index == 10)
        {
            minCost = 10;
            maxCost = int.MaxValue;
        }
        else
        {
            minCost = index;
            maxCost = index;
        }

        // 设置筛选器
        Filter.ClearFilters();
        Filter.AddFilter(new CostAmountFilter(minCost, maxCost));

        // 回到第一页
        CurrentPage = 1;

        // 应用筛选并刷新显示
        ApplyFilters();
        CloseCardInfo();
    }

    #region Timeline动画

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
    }

    #endregion

    #region UI跳转

    private void JumpToLobby()
    {
        if (DeckSystem.Instance.GetCurrentDeckCardCount() != 15)
        {
            DeckSystem.Instance.ClearCurrentDeck();
            CloseCardInfo();
        }
        else
        {
            DeckSystem.Instance.SaveDeck(DeckSystem.Instance.EditingDeckIndex, 
                DeckSystem.Instance.Decks[DeckSystem.Instance.EditingDeckIndex]);
        }

        CloseWnd();
        WndManager.Instance.OpenWnd<LobbyWnd>();
        WndManager.Instance.GetWnd<LobbyWnd>().ReDrawAnim();
    }

    #endregion

    #region 图鉴

    public void SetPage(int page)
    {
        if (page < 1) page = 1;
        if (page > TotalPages) page = TotalPages;
        CurrentPage = page;
        ApplyFilters();
    }

    public void NextPage()
    {
        SetPage(CurrentPage + 1);
    }

    public void PreviousPage()
    {
        SetPage(CurrentPage - 1);
    }

    public void ApplyFilters()
    {
        // 触发UI更新
        var currentCards = CurrentPageCards;
        for (int i = 0; i < mCards.Length; i++)
        {
            if (i < currentCards.Count)
            {
                // 有卡牌数据，初始化并显示
                mCards[i].gameObject.SetActive(true);
                mCards[i].Init(currentCards[i].CardID);
            }
            else
            {
                // 没有更多卡牌，隐藏
                mCards[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResetFilters()
    {
        Filter.ClearFilters();
        CurrentPage = 1;
        ApplyFilters();
    }

    #endregion

    #region 卡组

    public void RefreshDeckDisplay()
    {
        // 清空旧内容
        foreach (Transform child in Content)
        {
            child.gameObject.DestroySelf();
        }

        // 获取当前卡组的卡牌列表
        var deck = DeckSystem.Instance.Decks[DeckSystem.Instance.EditingDeckIndex];

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
            if (cardInfo == null) continue;

            GameObject go = Object.Instantiate(CardSlotInLibrary, Content);
            var slot = go.GetComponent<CardSlotInLibrary>();
            if (slot != null)
            {
                slot.Init(cardInfo);
            }
        }

        foreach (var card in mCards)
        {
            card.Varification();
        }

        mCardCountText.text = DeckSystem.Instance.GetCurrentDeckCardCount()+ " / <color=#9C2431>15";
    }

    #endregion

    #region 卡组介绍

    public void OpenCardInfo(int cardID)
    {
        DOTween.To(() => mGridLayoutGroup.spacing, x => mGridLayoutGroup.spacing = x, mOpenSpacing, 0.2f)
            .OnComplete(() =>
            {
                mDetail.gameObject.SetActive(true);
                mDetail.Init(this.SendQuery(new FindCardInfoWithIDQuery(cardID)));
            });
    }

    public void CloseCardInfo()
    {
        mDetail.gameObject.SetActive(false);
        DOTween.To(() => mGridLayoutGroup.spacing, x => mGridLayoutGroup.spacing = x, mCloseSpacing, 0.2f);
    }

    public void CloseCardInfoWwithNoAnimation()
    {
        mDetail.gameObject.SetActive(false);
        mGridLayoutGroup.spacing = mCloseSpacing;
    }

    #endregion

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return Inscription.Interface;
    }
}