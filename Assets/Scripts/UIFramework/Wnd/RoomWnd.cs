using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class RoomWnd : BaseWnd
{
    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;

    private Button mStartBtn;
    private TMP_Text mHostTip;
    private TMP_Text mClientTip;
    public Toggle mIsReadyToggle;
    private TMP_Text mIsReadyText;
    private TMP_Text mIsReadyTip;
    private RoomInfo mCurrentRoomInfo;
    private TMP_Text mTipText;


    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;

    public override void Initial()
    {
        //初始化组件


        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mStartBtn = SelfTransform.Find("StartBtn").GetComponent<Button>();
        mHostTip = SelfTransform.Find("HostTip").GetComponent<TextMeshProUGUI>();
        mClientTip = SelfTransform.Find("ClientTip").GetComponent<TextMeshProUGUI>();
        mIsReadyToggle = SelfTransform.Find("Toggle").GetComponent<Toggle>();
        mIsReadyText = SelfTransform.Find("Toggle/Label").GetComponent<TMP_Text>();
        mIsReadyTip = SelfTransform.Find("ReadyTip").GetComponent<TMP_Text>();
        mTipText = SelfTransform.Find("Tip_1").GetComponent<TMP_Text>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mStartBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(OnBackClick);
        mStartBtn.onClick.AddListener(OnStartClick);

        //绑定Toggle
        mIsReadyToggle.onValueChanged.AddListener(OnReadyToggle);
    }

    #region 房间相关

    private void OnBackClick()
    {
        JumpToRoomList();
        LobbyNet.Instance.Exitroom();
    }

    public void OnReadyToggle(bool ready)
    {
        if (ready)
        {
            mIsReadyText.text = "取消准备";
            LobbyNet.Instance.SyncRoomState(ready);
        }
        else
        {
            mIsReadyText.text = "准备";
            LobbyNet.Instance.SyncRoomState(ready);
        }
    }

    public void UpdateRoomState(bool clientPlayerReady)
    {
        if (clientPlayerReady)
        {
            mStartBtn.interactable = true;
            mIsReadyTip.text = "(已准备)";
        }
        else
        {
            mStartBtn.interactable = false;
            mIsReadyTip.text = "(未准备)";
        }
    }

    public void UpdateRoomInfo(RoomInfo roomInfo, PlayerInfo playerInfo)
    {
        mCurrentRoomInfo = roomInfo;
        // 先隐藏所有按钮
        mStartBtn.gameObject.SetActive(false);
        mIsReadyToggle.gameObject.SetActive(false);

        var mPlayerCount = roomInfo.HostPlayerID != "" && roomInfo.ClientPlayerID != "" ? 2 : 1;
        var mSlotCount = roomInfo.SlotCount;
        var mHealthCount = roomInfo.HealthCount;
        mTipText.text =
            $"C:/Lobby/Rooms:  房间信息: 人数 <color=#D7E2A3>{mPlayerCount}<color=#9C2431>/2  - 槽位 <color=#D7E2A3>{mSlotCount}<color=#9C2431> - 天平 <color=#D7E2A3>{mHealthCount}<color=#9C2431> - 套牌数 <color=#D7E2A3>15<color=#9C2431> -";
        mHostTip.text = roomInfo.HostPlayerName;
        mClientTip.text = roomInfo.ClientPlayerName;
        if (roomInfo.ClientPlayerID == "")
        {
            mIsReadyTip.text = "";
            mClientTip.text = "";
            mStartBtn.interactable = false;
        }
        else
        {
            if (roomInfo.HostPlayerID == playerInfo.PlayerID)
            {
                mStartBtn.interactable = false;
                mIsReadyTip.text = "(未准备)";
            }
            else
            {
                mIsReadyTip.text = "";
            }
        }

        if (roomInfo.HostPlayerID == playerInfo.PlayerID)
        {
            //为房主
            mStartBtn.gameObject.SetActive(true);
            mIsReadyToggle.gameObject.SetActive(false);
        }
        else if (roomInfo.ClientPlayerID == playerInfo.PlayerID)
        {
            mStartBtn.gameObject.SetActive(false);
            mIsReadyToggle.gameObject.SetActive(true);
        }
    }

    private void OnStartClick()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<GameWnd>();
        LobbyNet.Instance.SyncGameStart();
    }

    #endregion


    #region Timeline动画

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
        mIsReadyToggle.isOn = false;
        OnReadyToggle(false);
    }

    #endregion

    #region UI跳转

    private void JumpToRoomList()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<RoomListWnd>();
        WndManager.Instance.GetWnd<RoomListWnd>().ReDrawAnim();
    }

    #endregion
}