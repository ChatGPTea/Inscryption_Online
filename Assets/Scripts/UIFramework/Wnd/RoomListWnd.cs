using System.Collections.Generic;
using Inscryption;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class RoomListWnd : BaseWnd
{
    /// <summary>
    ///     初始化UI窗口，获取相对应的组件
    /// </summary>
    private Button mExitBtn;

    private Button mSlotAddBtn;
    private Button mSlotRemoveBtn;
    private Button mHPAddBtn;
    private Button mHPRemoveBtn;
    private Button mCountAddBtn;
    private Button mCountRemoveBtn;
    private Button mCreateBtn;
    private Button mJoinBtn;
    private Toggle mOtherToggle;
    private Transform mCountTransform;
    private TMP_Text mTipText;
    private TMP_Text mTip1Text;

    /// <summary>
    ///     Timeline组件
    /// </summary>
    private PlayableDirector mReDrawTimeline;


    /// <summary>
    /// 房间列表组件
    /// </summary>
    private GameObject mOriginCell;

    private List<RoomCell> mRoomCells = new List<RoomCell>();
    private RoomInfo mCurrentRoomInfo;
    private RoomListWndMono mMono;

    public override void Initial()
    {
        Debug.Log(LobbyNet.Instance);
        //初始化组件
        SelfTransform.gameObject.AddComponent<RoomListWndMono>();
        mMono = SelfTransform.GetComponent<RoomListWndMono>();

        //获取UI组件
        mExitBtn = SelfTransform.Find("ExitBtn").GetComponent<Button>();
        mJoinBtn = SelfTransform.Find("JoinBtn").GetComponent<Button>();
        mCreateBtn = SelfTransform.Find("CreateBtn").GetComponent<Button>();
        mSlotAddBtn = SelfTransform.Find("SlotSetting/LeftBtn").GetComponent<Button>();
        mSlotRemoveBtn = SelfTransform.Find("SlotSetting/RightBtn").GetComponent<Button>();
        mHPAddBtn = SelfTransform.Find("HPSetting/LeftBtn").GetComponent<Button>();
        mHPRemoveBtn = SelfTransform.Find("HPSetting/RightBtn").GetComponent<Button>();
        mCountAddBtn = SelfTransform.Find("CountSetting/LeftBtn").GetComponent<Button>();
        mCountRemoveBtn = SelfTransform.Find("CountSetting/RightBtn").GetComponent<Button>();
        mOtherToggle = SelfTransform.Find("OtherSetting").GetComponent<Toggle>();
        mCountTransform = SelfTransform.Find("Scroll View/Viewport/Content").GetComponent<Transform>();
        mTipText = SelfTransform.Find("Tip_1").GetComponent<TextMeshProUGUI>();
        mTip1Text = SelfTransform.Find("Tip_2").GetComponent<TextMeshProUGUI>();

        //获取Timeline
        mReDrawTimeline = SelfTransform.Find("DirectorReDraw").GetComponent<PlayableDirector>();

        //绑定Button音效
        mExitBtn.gameObject.AddComponent<CommandButtonMono>();
        mJoinBtn.gameObject.AddComponent<CommandButtonMono>();
        mCreateBtn.gameObject.AddComponent<CommandButtonMono>();

        //绑定Button跳转
        mExitBtn.onClick.AddListener(JumpToLobby);
        mCreateBtn.onClick.AddListener(OnCreatClick);
        mJoinBtn.onClick.AddListener(JoinRoom);
        mSlotAddBtn.onClick.AddListener(AddSlotCount);
        mSlotRemoveBtn.onClick.AddListener(RemoveSlotCount);
        mHPAddBtn.onClick.AddListener(AddHealthCount);
        mHPRemoveBtn.onClick.AddListener(RemoveHealthCount);

        //获取预制体,初始化房间列表
        mJoinBtn.interactable = false;
        UpdateRoomInfo();
        mOriginCell = Resources.Load<GameObject>("Prefabs/RoomInfoBtn");

        //第一次启动同步RoomList
        LobbyNet.Instance.SyncRoomList();
    }

    public void ConfirmCardsCount()
    {
        if (DeckSystem.Instance.GetCurrentDeckCardCount() == 15)
        {
            mTip1Text.text = "C:/Lobby/Player: 正在寻找<color=#D7E2A3> 旗鼓相当 <color=#9C2431>的对手";
        }
        else
        {
            mTip1Text.text = "C:/Lobby/Player: 你的套牌不足<color=#D7E2A3> 15 <color=#9C2431>张";
        }
    }

    #region 调整房间设置

    private void OnCreatClick()
    {
        if (DeckSystem.Instance.GetCurrentDeckCardCount() == 15)
        {
            Debug.Log(LobbyNet.Instance);
            LobbyNet.Instance.CreateRoom();
        }
    }

    private void AddSlotCount()
    {
        mMono.AddSlotCount();
    }

    private void RemoveSlotCount()
    {
        mMono.RemoveSlotCount();
    }

    private void AddHealthCount()
    {
        mMono.AddHealthCount();
    }

    private void RemoveHealthCount()
    {
        mMono.RemoveHealthCount();
    }

    #endregion

    #region 更新房间列表

    public void AddRoomCell(RoomInfo roomInfo)
    {
        GameObject clone = GameObject.Instantiate(mOriginCell, mCountTransform, false);
        RoomCell roomCell = clone.GetComponent<RoomCell>();
        mRoomCells.Add(roomCell);
        roomCell.Initial(roomInfo);
        clone.SetActive(true);
    }

    public void RemoveRoomCell(string roomID)
    {
        var roomcell = mRoomCells.Find(tmp => tmp.RoomInfo.RoomID == roomID);
        if (roomID == mCurrentRoomInfo.RoomID)
            UpdateRoomInfo();
        GameObject.Destroy(roomcell.gameObject);
        mRoomCells.Remove(roomcell);
    }

    public void UpdateRoomCell(RoomInfo roomInfo)
    {
        var targetCell = mRoomCells.Find(tmp => tmp.RoomInfo.RoomID == roomInfo.RoomID);
        targetCell.UpdateCell(roomInfo);
    }

    public void UpdateRoomInfo()
    {
        mCurrentRoomInfo = new RoomInfo();
        mTipText.text = $"C:/Lobby/Rooms:  房间信息: 暂无";
        mJoinBtn.interactable = false;
    }

    public void UpdateRoomInfo(RoomInfo roomInfo)
    {
        mCurrentRoomInfo = roomInfo;
        var mPlayerCount = roomInfo.HostPlayerID != "" && roomInfo.ClientPlayerID != "" ? 2 : 1;
        var mSlotCount = roomInfo.SlotCount;
        var mHealthCount = roomInfo.HealthCount;
        mTipText.text =
            $"C:/Lobby/Rooms:  房间信息: 人数 <color=#D7E2A3>{mPlayerCount}<color=#9C2431>/2  - 槽位 <color=#D7E2A3>{mSlotCount}<color=#9C2431> - 天平 <color=#D7E2A3>{mHealthCount}<color=#9C2431> - 套牌数 <color=#D7E2A3>15<color=#9C2431> -";
        if (mPlayerCount == 1)
        {
            mJoinBtn.interactable = true;
        }
        else
        {
            mJoinBtn.interactable = false;
        }
    }

    private void JoinRoom()
    {
        if (DeckSystem.Instance.GetCurrentDeckCardCount() == 15)
        {
            LobbyNet.Instance.JoinRoom(mCurrentRoomInfo);
        }
    }

    #endregion

    #region Timeline动画

    //刷新动画
    public void ReDrawAnim()
    {
        if (mReDrawTimeline.state == PlayState.Playing) mReDrawTimeline.Stop();
        mReDrawTimeline.Play();
        UpdateRoomInfo();
        ConfirmCardsCount();
    }

    #endregion

    #region UI跳转

    private void JumpToLobby()
    {
        CloseWnd();
        WndManager.Instance.OpenWnd<LobbyWnd>();
        WndManager.Instance.GetWnd<LobbyWnd>().ReDrawAnim();
    }

    #endregion
}