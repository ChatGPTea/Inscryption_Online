using Inscryption;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.UI;

public class LobbyWnd : BaseWnd
{
    private Button mCardList_1Btn;
    private Button mCardList_2Btn;
    private Button mCardList_3Btn;
    private Button mCardList_4Btn;

    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;

    private Button mMatchBtn;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    private Button mRoomBtn;
    private Button mSetCardBtn;
    private Button mSettingsBtn;

    private TMP_Text mNameTip;
    private TMP_Text mInfo;

    public override void Initial()
    {
        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mSettingsBtn = SelfTransform.Find("SettingsBtn").GetComponent<Button>();
        mRoomBtn = SelfTransform.Find("RoomBtn").GetComponent<Button>();
        mMatchBtn = SelfTransform.Find("MatchBtn").GetComponent<Button>();
        mSetCardBtn = SelfTransform.Find("SetCardBtn").GetComponent<Button>();
        mCardList_1Btn = SelfTransform.Find("CardList_1").GetComponent<Button>();
        mCardList_2Btn = SelfTransform.Find("CardList_2").GetComponent<Button>();
        mCardList_3Btn = SelfTransform.Find("CardList_3").GetComponent<Button>();
        mCardList_4Btn = SelfTransform.Find("CardList_4").GetComponent<Button>();
        mNameTip = SelfTransform.Find("NameTip").GetComponent<TMP_Text>();
        mInfo = SelfTransform.Find("Info").GetComponent<TMP_Text>();
        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mSettingsBtn.gameObject.AddComponent<CommandButtonMono>();
        mRoomBtn.gameObject.AddComponent<CommandButtonMono>();
        mMatchBtn.gameObject.AddComponent<CommandButtonMono>();
        mSetCardBtn.gameObject.AddComponent<CommandButtonMono>();
        mCardList_1Btn.gameObject.AddComponent<CommandButtonMono>();
        mCardList_2Btn.gameObject.AddComponent<CommandButtonMono>();
        mCardList_3Btn.gameObject.AddComponent<CommandButtonMono>();
        mCardList_4Btn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToStart);
        mRoomBtn.onClick.AddListener(JumpToRoomList);
        mSetCardBtn.onClick.AddListener(JumpToCardList);
        
        mNameTip.text = "C:/PlayerInfo: <color=#D7E2A3>" + ConfigManager.Instance.UserData.nickname;
        mInfo.text = "野兽卡组 " + DeckSystem.Instance.GetCurrentDeckCardCount() + " <color=#9C2431>/ 15";
    }

    #region Timeline动画

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
        
        mNameTip.text = "C:/PlayerInfo: <color=#D7E2A3>" + ConfigManager.Instance.UserData.nickname;
        mInfo.text = "野兽卡组 " + DeckSystem.Instance.GetCurrentDeckCardCount() + " <color=#9C2431>/ 15";
    }

    #endregion

    #region UI跳转

    private void JumpToStart()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<StartWnd>();
        WndManager.Instance.GetWnd<StartWnd>().ReDrawAnim();
    }

    private void JumpToRoomList()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<RoomListWnd>();
        WndManager.Instance.GetWnd<RoomListWnd>().ReDrawAnim();
    }

    private void JumpToCardList()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<CardListWnd>();
        WndManager.Instance.GetWnd<CardListWnd>().ReDrawAnim();
        WndManager.Instance.GetWnd<CardListWnd>().RefreshDeckDisplay();
    }

    #endregion
}