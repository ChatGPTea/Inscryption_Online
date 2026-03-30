using System.Collections.Generic;
using DG.Tweening;
using Inscryption;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameWnd : BaseWnd, ICanGetSystem, ICanGetModel
{
    public CardInHandManager CardInHandManager;
    public SlotsGroup mSlotsGroup;
    public GameMono mGameMono;

    //UI组件
    private Button mDrawMainCardBtn;
    private Button mDrawSecondBtn;
    private Button mOptBtn;
    private TMP_Text mTip;
    private Button mPlayerTurnEndBtn;
    private TMP_Text mTextTip;
    public TMP_Text mPlayerName;
    public TMP_Text mEnemyName;
    public string PlayerName;
    public string EnemyName;
    private Button FuckMessager;
    private Button WOWMessager;
    private Button HelloMessager;
    private Toggle PlayerMessager;
    private Transform EnemyMessager;
    private Transform MessageContent;

    //卡牌信息UI组件
    private Transform mCardInfoTip;
    private Text mName_1;
    private Text mName_2;
    private Text mContext_1;
    private Text mContext_2;

    private CardInHand mCardObj;

    //献祭相关UI组件
    private Transform mCountTip;
    private Image mCountTipBG;
    private Transform mCountIcon;

    //SO文件
    private CardToolTip_SO mCardToolTip_SO;
    private CountInfo mCountInfo;

    //献祭相关变量
    public int CurrentCardID = -1;
    public CardInHandData CurrentCardData;
    public int mCost = -1000;
    public bool isReady = false;
    private float mCurrentPosition;
    public List<CardInSlot> SacrificeList = new List<CardInSlot>();

    //场上格子和造物相关
    public Transform EnemySlotGroup;
    public Slot[] EnemySlots;

    //开局先后手相关
    private Transform mPlayerPoint;
    private Transform mEnemyPoint;
    private ITurnSystem mTurnSystem;
    private ICardSystem mCardSystem;
    public EasyEvent mGameBeginEvent = new EasyEvent();

    public override void Initial()
    {
        GameManager.Instance.TurnOnAndDownLight(true);
        //初始化牌桌 处理卡槽逻辑
        SelfTransform.gameObject.AddComponent<SlotsGroup>();
        mSlotsGroup = SelfTransform.GetComponent<SlotsGroup>();

        //初始化抽卡 处理抽卡逻辑
        SelfTransform.gameObject.AddComponent<CardInHandManager>();
        CardInHandManager = SelfTransform.GetComponent<CardInHandManager>();

        //初始化回合相关
        SelfTransform.gameObject.AddComponent<GameMono>();
        mGameMono = SelfTransform.GetComponent<GameMono>();
        mGameMono.Init();
        mTurnSystem = this.GetSystem<ITurnSystem>();
        mCardSystem = this.GetSystem<ICardSystem>();
        this.GetModel<ISlotsModel>().ReSetSlots(this.GetModel<IRoomModel>().SlotCount.Value);
        mCardSystem.SetPlayerCard();
        //初始化组件
        mOptBtn = SelfTransform.Find("OptBtn").GetComponent<Button>();
        mDrawMainCardBtn = SelfTransform.Find("MainCards/Card_1").GetComponent<Button>();
        mDrawSecondBtn = SelfTransform.Find("SecondCards/Card_1").GetComponent<Button>();
        mPlayerTurnEndBtn = SelfTransform.Find("BellBtn").GetComponent<Button>();
        mTip = SelfTransform.Find("Tip").GetComponent<TMP_Text>();
        mCardInfoTip = SelfTransform.Find("CardInfoTip");
        mName_1 = SelfTransform.Find("CardInfoTip/Tip_1_2/Right/Name_1").GetComponent<Text>();
        mName_2 = SelfTransform.Find("CardInfoTip/Tip_1_2/Right/Name_2").GetComponent<Text>();
        mContext_1 = SelfTransform.Find("CardInfoTip/Tip_1_2/Right/Context_1").GetComponent<Text>();
        mContext_2 = SelfTransform.Find("CardInfoTip/Tip_1_2/Right/Context_2").GetComponent<Text>();
        mCardObj = SelfTransform.Find("CardInfoTip/CardInfoTip").GetComponent<CardInHand>();
        mCountTip = SelfTransform.Find("CardInfoTip/CountTip").GetComponent<Transform>();
        mCountTipBG = SelfTransform.Find("CardInfoTip/CountTip/BG").GetComponent<Image>();
        mCountIcon = SelfTransform.Find("CardInfoTip/CountTip/Icon").GetComponent<Transform>();
        EnemySlotGroup = SelfTransform.Find("EnemySlotGroup").GetComponent<Transform>();
        mPlayerPoint = SelfTransform.Find("PlayerPoint");
        mEnemyPoint = SelfTransform.Find("EnemyPoint");
        mTextTip = SelfTransform.Find("TextTip/Text").GetComponent<TMP_Text>();
        mPlayerName = SelfTransform.Find("PlayerName").GetComponent<TMP_Text>();
        mEnemyName = SelfTransform.Find("EnemyName").GetComponent<TMP_Text>();

        //放狠话环节
        FuckMessager = SelfTransform.Find("Messagers/FuckMessager").GetComponent<Button>();
        WOWMessager = SelfTransform.Find("Messagers/WOWMessager").GetComponent<Button>();
        HelloMessager = SelfTransform.Find("Messagers/HelloMessager").GetComponent<Button>();
        PlayerMessager = SelfTransform.Find("PlayerMessager").GetComponent<Toggle>();
        MessageContent = SelfTransform.Find("MessageContent");
        EnemyMessager = SelfTransform.Find("EnemyMessager");
        
        FuckMessager.onClick.AddListener(PressFuckMessage);
        WOWMessager.onClick.AddListener(PressWOWMessage);
        HelloMessager.onClick.AddListener(PressHelloMessage);
        PlayerMessager.onValueChanged.AddListener(SelectSendMessage);

        mCurrentPosition = mCountTip.transform.position.y;

        //获取So文件
        mCardToolTip_SO = Resources.Load("SO/Ability/印记信息") as CardToolTip_SO;
        mCountInfo = Resources.Load("SO/Count/费用图片") as CountInfo;

        mOptBtn.onClick.AddListener(OpenOptWnd);
        mDrawMainCardBtn.onClick.AddListener(PlayerDrawMainCard);
        mDrawSecondBtn.onClick.AddListener(PlayerDrawSecondCard);
        mPlayerTurnEndBtn.onClick.AddListener(PlayerEndTurn);
    }

    #region 回合状态

    //玩家回合结束
    private void PlayerEndTurn()
    {
        AudioKit.PlaySound("resources://Sound/Cards/combatbell_ring");
        if (mTurnSystem.State.Value == TurnState.EnemyTurn)
        {
            TryPressAtEnemyTurn();
        }
        else if (mTurnSystem.State.Value == TurnState.PlayerTurnBegin)
        {
            TryPressBeforeDrawCard();
        }

        if (isReady) return;

        if (mTurnSystem.State.Value == TurnState.PlayerFirstTurnBegin ||
            mTurnSystem.State.Value == TurnState.AfterDrawCard)
        {
            HideCardInfoTip();
            GameNet.Instance.SyncTurn();
            mGameMono.PlayerAttack();
        }
    }

    //开局先后手
    public void DecideWhoFirst(bool isFirst)
    {
        mPlayerPoint.gameObject.SetActive(true);
        mEnemyPoint.gameObject.SetActive(true);
        YouAreFirstOrNot(isFirst);
        if (isFirst)
        {
            mPlayerPoint.Find("First").gameObject.SetActive(true);
            mPlayerPoint.Find("NotFirst").gameObject.SetActive(false);
            mEnemyPoint.Find("First").gameObject.SetActive(false);
            mEnemyPoint.Find("NotFirst").gameObject.SetActive(true);
        }
        else
        {
            mPlayerPoint.Find("First").gameObject.SetActive(false);
            mPlayerPoint.Find("NotFirst").gameObject.SetActive(true);
            mEnemyPoint.Find("First").gameObject.SetActive(true);
            mEnemyPoint.Find("NotFirst").gameObject.SetActive(false);
        }

        var mplayerAnim = mPlayerPoint.gameObject.GetComponent<Animator>();
        var mEnemyAnim = mEnemyPoint.gameObject.GetComponent<Animator>();
        mplayerAnim.Play("JumpInto");
        mEnemyAnim.Play("EnemyJumpInto");
        ChangeTextTip("对战开始");

        this.GetSystem<ITimeSystem>().AddDelayTask(1f, () =>
        {
            mPlayerPoint.Find("PlayerName").gameObject.SetActive(true);
            mEnemyPoint.Find("EnemyName").gameObject.SetActive(true);
            mPlayerPoint.Find("PlayerName").gameObject.GetComponent<TMP_Text>().text =
                LobbyNet.Instance.LocalPlayerInfo.PlayerName;
            if (LobbyNet.Instance.IsHostPlayer())
            {
                mEnemyPoint.Find("EnemyName").gameObject.GetComponent<TMP_Text>().text =
                    LobbyNet.Instance.LocalRoomInfo.ClientPlayerName;
            }
            else
            {
                mEnemyPoint.Find("EnemyName").gameObject.GetComponent<TMP_Text>().text =
                    LobbyNet.Instance.LocalRoomInfo.HostPlayerName;
            }
        });

        this.GetSystem<ITimeSystem>().AddDelayTask(2.5f, () =>
        {
            mGameBeginEvent.Trigger();
            mPlayerPoint.gameObject.SetActive(false);
            mEnemyPoint.gameObject.SetActive(false);
            mPlayerName.text = LobbyNet.Instance.LocalPlayerInfo.PlayerName;
            PlayerName = LobbyNet.Instance.LocalPlayerInfo.PlayerName;
            if (isFirst) mPlayerName.text = PlayerName + " 行动中";
            if (LobbyNet.Instance.IsHostPlayer())
            {
                mEnemyName.text = LobbyNet.Instance.LocalRoomInfo.ClientPlayerName;
                EnemyName = LobbyNet.Instance.LocalRoomInfo.ClientPlayerName;
                if (!isFirst) mEnemyName.text = PlayerName + " 行动中";
            }
            else
            {
                mEnemyName.text = LobbyNet.Instance.LocalRoomInfo.HostPlayerName;
                EnemyName = LobbyNet.Instance.LocalRoomInfo.HostPlayerName;
                if (!isFirst) mEnemyName.text = PlayerName + " 行动中";
            }
        });

        this.GetSystem<ITimeSystem>().AddDelayTask(1.5f, () =>
        {
            mPlayerPoint.DOPunchScale(Vector2.one * 0.1f, 0.3f);
            mEnemyPoint.DOPunchScale(Vector2.one * 0.1f, 0.3f);
            if (isFirst)
            {
                mPlayerPoint.Find("FirstTip").gameObject.SetActive(true);
                mPlayerPoint.Find("FirstTip").DOPunchScale(Vector2.one * 0.2f, 0.3f);
            }
            else
            {
                mEnemyPoint.Find("FirstTip").gameObject.SetActive(true);
                mEnemyPoint.Find("FirstTip").DOPunchScale(Vector2.one * 0.2f, 0.3f);
            }
        });
        this.GetSystem<ITimeSystem>().AddDelayTask(3.5f, () => { CardInHandManager.PlayerDrawMainCard(); });
        this.GetSystem<ITimeSystem>().AddDelayTask(3.7f, () => { CardInHandManager.PlayerDrawMainCard(); });
        this.GetSystem<ITimeSystem>().AddDelayTask(3.9f, () => { CardInHandManager.PlayerDrawMainCard(); });
        this.GetSystem<ITimeSystem>().AddDelayTask(4.1f, () =>
        {
            if (isFirst)
            {
                CardInHandManager.PlayerDrawSecondCard();
                mTurnSystem.State.Value = TurnState.PlayerFirstTurnBegin;
            }
            else
            {
                mTurnSystem.State.Value = TurnState.EnemyTurn;
            }
        });
    }

    #endregion

    #region 抽卡

    private void PlayerDrawSecondCard()
    {
        //提示
        if (mTurnSystem.State.Value == TurnState.PlayerFirstTurnBegin)
        {
            TryDrawSecondCardBeginFirstTurn();
        }

        if (mTurnSystem.State.Value == TurnState.EnemyTurn)
        {
            TryDrawCardAtEnemyTurn();
        }

        if (mTurnSystem.State.Value == TurnState.AfterDrawCard)
        {
            TryDrawCardAgain();
        }

        if (mTurnSystem.State.Value == TurnState.PlayerTurnBegin)
        {
            CardInHandManager.PlayerDrawSecondCard();
            mTurnSystem.State.Value = TurnState.AfterDrawCard;
        }
    }

    private void PlayerDrawMainCard()
    {
        //提示
        if (mTurnSystem.State.Value == TurnState.PlayerFirstTurnBegin)
        {
            TryDrawMainCardBeginFirstTurn();
        }

        if (mTurnSystem.State.Value == TurnState.EnemyTurn)
        {
            TryDrawCardAtEnemyTurn();
        }

        if (mTurnSystem.State.Value == TurnState.AfterDrawCard)
        {
            TryDrawCardAgain();
        }

        if (mTurnSystem.State.Value == TurnState.PlayerTurnBegin)
        {
            CardInHandManager.PlayerDrawMainCard();
            mTurnSystem.State.Value = TurnState.AfterDrawCard;
        }
    }

    #endregion

    #region 显示卡牌详情

    /// <summary>
    /// 显示卡牌信息
    /// </summary>
    public void ReDrawCardInfoTip(GameObject card,bool isWatchingEnemyCard)
    {
        mCardInfoTip.gameObject.SetActive(true);

        int cardID = card.GetComponent<CardInHand>().mCurrentCardID;
        var cardData = card.GetComponent<CardInHand>().mData;
        mCardObj.Init(cardID, cardData);

        if(!isWatchingEnemyCard)
            ShowCountTip(cardData);

        if (cardData.Abilities.Count == 0)
        {
            mName_1.gameObject.SetActive(false);
            mContext_1.gameObject.SetActive(false);
            mName_2.gameObject.SetActive(false);
            mContext_2.gameObject.SetActive(false);
            
        }
        else if (cardData.Abilities.Count == 1)
        {
            //控制组件开关
            mName_1.gameObject.SetActive(true);
            mContext_1.gameObject.SetActive(true);
            mName_2.gameObject.SetActive(false);
            mContext_2.gameObject.SetActive(false);

            if (mCardToolTip_SO == null) Debug.Log(1);
            var ability_1 = mCardToolTip_SO.GetCardAbilityTip(cardData.Abilities[0]);
            mName_1.text = ability_1.abliltyName;
            mContext_1.text = ability_1.description;
        }
        else if (cardData.Abilities.Count == 2)
        {
            //控制组件开关
            mName_1.gameObject.SetActive(true);
            mContext_1.gameObject.SetActive(true);
            mName_2.gameObject.SetActive(true);
            mContext_2.gameObject.SetActive(true);

            var ability_1 = mCardToolTip_SO.GetCardAbilityTip(cardData.Abilities[0]);
            var ability_2 = mCardToolTip_SO.GetCardAbilityTip(cardData.Abilities[1]);
            mName_1.text = ability_1.abliltyName;
            mContext_1.text = ability_1.description;
            mName_2.text = ability_2.abliltyName;
            mContext_2.text = ability_2.description;

            mName_2.gameObject.SetActive(true);
            mContext_2.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏卡牌信息，清空信息
    /// </summary>
    public void HideCardInfoTip()
    {
        DestroyCost();
        mCardInfoTip.gameObject.SetActive(false);
    }

    public void HideCardInfoTip(bool isSuccess)
    {
        DestroyCost(isSuccess);
        mCardInfoTip.gameObject.SetActive(false);
    }

    public void ShowCountTip(CardInHandData cardData)
    {
        DestroyCost();
        if (cardData.Cost == 0)
        {
            mCost = cardData.Cost;
            mCountTip.gameObject.SetActive(true);
            mCountIcon.gameObject.SetActive(true);
            mCountTipBG.sprite = mCountInfo.mCountCost[mCost];
            CurrentCardID = cardData.CardId;
            CurrentCardData = cardData;
            isReady = true;
        }
        else
        {
            mCost = cardData.Cost;
            mCountTip.gameObject.SetActive(true);
            mCountIcon.gameObject.SetActive(false);
            mCountTipBG.sprite = mCountInfo.mCountCost[mCost];
            CurrentCardID = cardData.CardId;
            CurrentCardData = cardData;
        }
    }

    public void ReduceCost(int Value)
    {
        mCost -= Value;
        mCountTip.DOKill();
        mCountTip.rotation = Quaternion.Euler(0, 0, 0);
        Sequence sequence = DOTween.Sequence();

        // 向上移动同时旋转
        sequence.Append(mCountTip.DOMoveY(mCurrentPosition + 0.3f, 0.1f).SetEase(Ease.OutQuad));
        sequence.Join(mCountTip.DORotate(new Vector3(0, 180, 360), 0.1f, RotateMode.LocalAxisAdd));

        if (mCost <= 0)
        {
            mCountTip.gameObject.SetActive(true);
            mCountIcon.gameObject.SetActive(true);
            mCountTipBG.sprite = mCountInfo.mCountCost[0];
            SuccSacrifice();
            isReady = true;
        }
        else
        {
            mCountTip.gameObject.SetActive(true);
            mCountIcon.gameObject.SetActive(false);
            mCountTipBG.sprite = mCountInfo.mCountCost[mCost];
        }

        // 向下移动并继续旋转
        sequence.Append(mCountTip.DOMoveY(mCurrentPosition, 0.1f).SetEase(Ease.InQuad));
        sequence.Join(mCountTip.DORotate(new Vector3(0, 180, 0), 0.1f, RotateMode.LocalAxisAdd))
            .OnComplete(() => { mCountTip.rotation = Quaternion.Euler(0, 0, 0); });
    }

    //正常关闭
    private void DestroyCost()
    {
        CurrentCardID = -1;
        mCost = -1000;
        CurrentCardData = new CardInHandData();
        isReady = false;
        mCountTip.rotation = Quaternion.Euler(0, 0, 0);

        for (var i = SacrificeList.Count - 1; i >= 0; i--)
        {
            var cardNeedSacrifice = SacrificeList[i];
            cardNeedSacrifice.QuitSacrifice();
        }
    }

    //成功献祭
    private void DestroyCost(bool isSuccess)
    {
        CurrentCardID = -1;
        mCost = -1000;
        CurrentCardData = new CardInHandData();
        isReady = false;
    }

    private void SuccSacrifice()
    {
        for (var i = SacrificeList.Count - 1; i >= 0; i--)
        {
            var cardNeedSacrifice = SacrificeList[i];
            int cardID = cardNeedSacrifice.transform.parent.parent.gameObject.GetComponent<Slot>().slotIndex;
            cardNeedSacrifice.QuitSacrifice(true);
            cardNeedSacrifice.mCurrentState = CardInSlotState.Idle;
            GameNet.Instance.SyncSacrifice(cardID);
        }
    }

    #endregion

    #region 牌堆相关

    public void ReDrawCardCount(int count1, int count2)
    {
        mTip.text = "<color=#D7E2A3>" + count1 + "<color=#9C232E>/" + count2;
    }

    #endregion

    #region UI跳转

    private void OpenOptWnd()
    {
        WndManager.Instance.OpenWnd<TextWnd>();
        WndManager.Instance.GetWnd<TextWnd>().SetAsFirst();
    }

    #endregion

    #region 文字提示

    /// <summary>
    /// 先后手
    /// </summary>
    public void YouAreFirstOrNot(bool isFirst)
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "calm (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        if (isFirst)
        {
            int randomNumber = Random.Range(0, 3);
            switch (randomNumber)
            {
                case 0:
                    ChangeTextTip("你握着优先下牌的权力...");
                    break;
                case 1:
                    ChangeTextTip("你先请...");
                    break;
                case 2:
                    ChangeTextTip("先手之利...");
                    break;
            }
        }
        else
        {
            int randomNumber = Random.Range(0, 3);
            switch (randomNumber)
            {
                case 0:
                    ChangeTextTip("迟来的步伐...往往能踩在最致命的地方");
                    break;
                case 1:
                    ChangeTextTip("你静静等待反击的时机");
                    break;
                case 2:
                    ChangeTextTip("你是后手...");
                    break;
            }
        }
    }

    /// <summary>
    /// 第一回合尝试抽主卡组
    /// </summary>
    public void TryDrawMainCardBeginFirstTurn()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "frustrated (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("静候...");
                break;
            case 1:
                ChangeTextTip("你感觉有人在盯着你...");
                break;
            case 2:
                ChangeTextTip("停");
                break;
        }
    }

    /// <summary>
    /// 第一回合尝试抽副卡组
    /// </summary>
    public void TryDrawSecondCardBeginFirstTurn()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "frustrated (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("不行...");
                break;
            case 1:
                ChangeTextTip("停.");
                break;
            case 2:
                ChangeTextTip("第一回合不能抽牌，你不知道吗？");
                break;
        }
    }

    /// <summary>
    /// 敌方回合抽牌
    /// </summary>
    public void TryDrawCardAtEnemyTurn()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "frustrated (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("静待佳音...");
                break;
            case 1:
                ChangeTextTip("造物只会在你的回合回应你");
                break;
            case 2:
                ChangeTextTip("造物总会闻声而至...");
                break;
        }
    }

    /// <summary>
    /// 未抽卡前尝试按铃铛
    /// </summary>
    public void TryPressBeforeDrawCard()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "frustrated (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("在卡牌尚未入手之前，铃铛不会回应你");
                break;
            case 1:
                ChangeTextTip("先抽卡.");
                break;
            case 2:
                ChangeTextTip("迫不及待的突袭...容我拒绝...");
                break;
        }
    }

    /// <summary>
    /// 敌方回合时尝试按铃铛
    /// </summary>
    public void TryPressAtEnemyTurn()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "frustrated (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("你的回合还未到来，铃铛不会回应你");
                break;
            case 1:
                ChangeTextTip("保持静默.");
                break;
            case 2:
                ChangeTextTip("不行...容我拒绝...");
                break;
        }
    }

    /// <summary>
    /// 尝试将造物摆放在敌方格子上
    /// </summary>
    public void TryPlaceCardAtEnemySlot()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "laughing (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("把它放在你的格子上…");
                break;
            case 1:
                ChangeTextTip("这个位置不对...");
                break;
            case 2:
                ChangeTextTip("你如此慷慨，而我拒绝…");
                break;
        }
    }

    /// <summary>
    /// 主卡组清空时抽卡
    /// </summary>
    public void TryDrawCardWhenCardListEmpty()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "laughing (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("空荡的山谷中全无造物...");
                break;
            case 1:
                ChangeTextTip("饥饿的感觉传遍全身...松鼠成了你最后的口粮");
                break;
            case 2:
                ChangeTextTip("牌库空空如也...我也无能为力");
                break;
        }
    }

    /// <summary>
    /// 尝试额外抽卡
    /// </summary>
    public void TryDrawCardAgain()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "laughing (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("已经有一只造物屈尊加入了你的手牌...");
                break;
            case 1:
                ChangeTextTip("未加载“左右开弓之奖”");
                break;
            case 2:
                ChangeTextTip("差不多就得了啊");
                break;
        }
    }

    /// <summary>
    /// 在没有足够祭品时试图召唤
    /// </summary>
    public void TryPlaceWhenNotReady(string name, int cost)
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "laughing (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("你的祭品不足");
                break;
            case 1:
                ChangeTextTip("它并不满意于这点酬劳”");
                break;
            case 2:
                ChangeTextTip($"想要打出 {name} 仍需 {cost} 个祭品");
                break;
        }
    }

    /// <summary>
    /// 试图献祭敌方牌
    /// </summary>
    public void TryUseEnemyCardInSlot()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "laughing (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("你应该在自己的牌桌寻找祭品");
                break;
            case 1:
                ChangeTextTip("它拒绝为你而死");
                break;
            case 2:
                ChangeTextTip("来自敌方的祭品吗...不行");
                break;
        }
    }

    /// <summary>
    ///  己方回合开始
    /// </summary>
    public void PlayerTurnBegin()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "calm (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("到你了...");
                break;
            case 1:
                ChangeTextTip("你的回合");
                break;
            case 2:
                ChangeTextTip("轮到你行动了");
                break;
        }
    }

    /// <summary>
    /// 胜利
    /// </summary>
    public void PlayerWinGame()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "calm (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("未登顶峰...");
                break;
            case 1:
                ChangeTextTip("一场鲜血淋漓的旅途");
                break;
            case 2:
                ChangeTextTip("保持...前进");
                break;
        }
    }

    /// <summary>
    /// 失败
    /// </summary>
    public void PlayerLoseGame()
    {
        var _randomNum = Random.Range(1, 10);
        string soundName = "calm (" + _randomNum + ")";
        AudioKit.PlaySound("resources://Sound/P03/" + soundName);
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ChangeTextTip("胜利属于你的对手...");
                break;
            case 1:
                ChangeTextTip("好了，拿上蜡烛，离开这里");
                break;
            case 2:
                ChangeTextTip("烛火已灭");
                break;
        }
    }

    public void ChangeTextTip(string text)
    {
        if (mTextTip.transform.parent.transform.gameObject.activeSelf)
        {
            mTextTip.transform.parent.transform.GetComponent<ResettableTimer>().ResetAndStartTimer();
        }
        else
        {
            mTextTip.transform.parent.transform.gameObject.SetActive(true);
        }

        mTextTip.text = "<shake>" + text + "<@shake>";
    }

    #endregion

    #region 放狠话环节

    public void SelectSendMessage(bool isOn)
    {
        if (isOn)
        {
            FuckMessager.gameObject.SetActive(true);
            HelloMessager.gameObject.SetActive(true);
            WOWMessager.gameObject.SetActive(true);
            FuckMessager.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            HelloMessager.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            WOWMessager.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            FuckMessager.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetEase(Ease.InOutElastic);
            HelloMessager.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetEase(Ease.InOutElastic);
            WOWMessager.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetEase(Ease.InOutElastic);
        }
        else
        {
            FuckMessager.gameObject.SetActive(false);
            HelloMessager.gameObject.SetActive(false);
            WOWMessager.gameObject.SetActive(false);
        }
    }

    public void PressHelloMessage()
    {
        SelectSendMessage(false);
        PlayerMessager.isOn = false;
        PlayerMessager.interactable = false;
        UnlockMessage();
        
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ActiveMyContent("你好...旅行者");
                break;
            case 1:
                ActiveMyContent("祝你好运");
                break;
            case 2:
                ActiveMyContent("生有涯，艺无涯");
                break;
        }
    }

    public void PressFuckMessage()
    {
        SelectSendMessage(false);
        PlayerMessager.isOn = false;
        PlayerMessager.interactable = false;
        UnlockMessage();
        
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ActiveMyContent("还打...？");
                break;
            case 1:
                ActiveMyContent("会不会玩啊你");
                break;
            case 2:
                ActiveMyContent("你在看什么？");
                break;
        }
    }

    public void PressWOWMessage()
    {
        SelectSendMessage(false);
        PlayerMessager.isOn = false;
        PlayerMessager.interactable = false;
        UnlockMessage();
        
        int randomNumber = Random.Range(0, 3);
        switch (randomNumber)
        {
            case 0:
                ActiveMyContent("哇偶");
                break;
            case 1:
                ActiveMyContent("我没想到这个");
                break;
            case 2:
                ActiveMyContent("这...");
                break;
        }
    }

    private void ActiveMyContent(string content)
    {
        MessageContent.gameObject.SetActive(true);
        MessageContent.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        MessageContent.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetEase(Ease.InOutElastic);
        MessageContent.Find("Tip").GetComponent<TMP_Text>().text = content;
        this.GetSystem<ITimeSystem>().AddDelayTask(1.5f,
            () =>
            {
                MessageContent.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutElastic).OnComplete(() =>
                {
                    MessageContent.gameObject.SetActive(false);
                });
            });
        GameNet.Instance.SyncMessage(content);
    }

    public void ActiveEnemyContent(string content)
    {
        EnemyMessager.gameObject.SetActive(true);
        EnemyMessager.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        EnemyMessager.DOScale(new Vector3(0.3f, 0.3f, 0.3f), 0.3f).SetEase(Ease.InOutElastic);
        EnemyMessager.Find("Tip").GetComponent<TMP_Text>().text = content;
        this.GetSystem<ITimeSystem>().AddDelayTask(1.5f,
            () =>
            {
                EnemyMessager.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InOutElastic).OnComplete(() =>
                {
                    EnemyMessager.gameObject.SetActive(false);
                });
            });
    }

    private void UnlockMessage()
    {
        this.GetSystem<ITimeSystem>().AddDelayTask(2f, () => { PlayerMessager.interactable = true; });
    }

    #endregion


    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return Inscription.Interface;
    }
}