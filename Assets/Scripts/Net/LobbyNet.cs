using System;
using System.Collections.Generic;
using Inscryption;
using QFramework;
using Unity.Netcode;
using UnityEngine;

public class LobbyNet : NetworkBehaviour, ICanGetModel, ICanGetSystem
{
    public static LobbyNet Instance;
    public IRoomModel mRoomModel;

    //客户端管理变量
    private Dictionary<string, RoomInfo> _roomInfoDict;
    private Dictionary<string, RoomInfo> _matchInfoDict;
    private Dictionary<string, PlayerInfo> _playerInfoDict;

    //本地信息
    public PlayerInfo LocalPlayerInfo;
    public RoomInfo LocalRoomInfo;

    private Dictionary<ulong, string> _netIDToPlayerID; // NetID -> PlayerID 映射
    
    

    void Start()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            _roomInfoDict = new Dictionary<string, RoomInfo>();
            _matchInfoDict = new Dictionary<string, RoomInfo>();
            _playerInfoDict = new Dictionary<string, PlayerInfo>();
            _netIDToPlayerID = new Dictionary<ulong, string>(); // ← 新增
            Dictionary<string, string> formData = new Dictionary<string, string>();
            HTTPHandler.Instance.POST("user/force-logout-all", formData, OnForceLogoutAllSuccess, OnForceLogoutAllFailed);
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
        else
        {
            LocalPlayerInfo = new PlayerInfo()
            {
                PlayerNetID = NetworkManager.LocalClientId,
                PlayerID = ConfigManager.Instance.UserData.id,
                PlayerName = ConfigManager.Instance.UserData.nickname,
                RoomID = "",
            };
            AddPlayerInfoRpc(LocalPlayerInfo);
            // 客户端注册断线回调
            NetworkManager.OnClientDisconnectCallback  += OnClientDisconnect_ShowUI;
        }
    }

    private void OnClientDisconnect_ShowUI(ulong obj)
    {
        WndManager.Instance.OpenWnd<LostWnd>();
    }

    void OnForceLogoutAllSuccess(string msg)
    {
        Debug.Log($"[ForceLogoutAll] 成功: {msg}");
    }

    void OnForceLogoutAllFailed(string error)
    {
        Debug.LogError($"[ForceLogoutAll] 失败: {error}");
    }
    

    [Rpc(SendTo.Server)]
    void AddPlayerInfoRpc(PlayerInfo playerInfo)
    {
        _playerInfoDict[playerInfo.PlayerID] = playerInfo;
        _netIDToPlayerID[playerInfo.PlayerNetID] = playerInfo.PlayerID; // ← 新增映射
        Debug.Log(
            $"[Server] PlayerJoinIn: {playerInfo.PlayerName}, NetID: {playerInfo.PlayerNetID}, PlayerID: {playerInfo.PlayerID}");
    }

    [Rpc(SendTo.Server)]
    void UpdatePlayerInfoRpc(PlayerInfo playerInfo)
    {
        _playerInfoDict[playerInfo.PlayerID] = playerInfo;
    }

    #region 退出

    private void NetworkManager_OnClientDisconnectCallback(ulong clientNetID)
    {
        Debug.Log($"[Server] 客户端断开连接: NetID={clientNetID}");

        if (!_netIDToPlayerID.TryGetValue(clientNetID, out string playerID))
        {
            Debug.LogWarning($"[Server] 无法找到对应PlayerID, NetID: {clientNetID}");
            return;
        }

        if (!_playerInfoDict.TryGetValue(playerID, out PlayerInfo playerInfo))
        {
            Debug.LogWarning($"[Server] 无法找到PlayerInfo, PlayerID: {playerID}");
            return;
        }

        Debug.Log($"[Server] 玩家断开: {playerInfo.PlayerName}, RoomID: {playerInfo.RoomID}");
        Dictionary<string, string> formData = new Dictionary<string, string>();
        formData.Add("userid", playerInfo.PlayerID);
        HTTPHandler.Instance.POST("user/logout", formData, msg => { }, msg => { });
        // 先处理房间逻辑
        ClearRoom(playerInfo);
        // 从内存中移除
        _playerInfoDict.Remove(playerID);
        _netIDToPlayerID.Remove(clientNetID);
        
    }

    void ClearRoom(PlayerInfo playerInfo)
    {
        RoomInfo roomInfo = default;
        foreach (var item in _roomInfoDict)
        {
            if (item.Value.RoomID == playerInfo.RoomID)
            {
                roomInfo = item.Value;
                break;
            }
        }
        
        // 判断是房主还是客机
        bool isHost = playerInfo.PlayerID == roomInfo.HostPlayerID;
        bool isClient = playerInfo.PlayerID == roomInfo.ClientPlayerID;

        if (!isHost && !isClient)
        {
            Debug.LogWarning("[Server] 玩家既不是房主也不是客机，无需处理");
            return;
        }
        
        if (isHost)
        {
            // 房主断开
            if (!string.IsNullOrEmpty(roomInfo.ClientPlayerID))
            {
                // 客机升为房主
                roomInfo.HostPlayerID = roomInfo.ClientPlayerID;
                roomInfo.HostPlayerName = roomInfo.ClientPlayerName;
                roomInfo.HostPlayerNetID = roomInfo.ClientPlayerNetID;
                roomInfo.ClientPlayerID = "";
                roomInfo.ClientPlayerName = "";
                roomInfo.ClientPlayerNetID = 0;

                _roomInfoDict[roomInfo.RoomID] = roomInfo;
                ExitRoomRpc(roomInfo);
                Debug.Log($"[Server] 房主断开，客机 {roomInfo.HostPlayerName} 成为新房主");
            }
            else
            {
                // 没有客机，解散房间
                _roomInfoDict.Remove(roomInfo.RoomID);
                RemoveRoomServerRpc(roomInfo.RoomID);
                Debug.Log($"[Server] 房主断开，房间 {roomInfo.RoomID} 解散");
            }
        }
        else
        {
            // 客机断开
            roomInfo.ClientPlayerID = "";
            roomInfo.ClientPlayerName = "";
            roomInfo.ClientPlayerNetID = 0;

            _roomInfoDict[roomInfo.RoomID] = roomInfo;
            ExitRoomRpc(roomInfo);
            Debug.Log($"[Server] 客机断开，房主 {roomInfo.HostPlayerName} 获胜");
        }
    }

    #endregion


    [Rpc(SendTo.ClientsAndHost)]
    void RemoveRoomServerRpc(string roomID)
    {
        var wnd = WndManager.Instance.GetWnd<RoomListWnd>();
        if (wnd != null)
        {
            wnd.RemoveRoomCell(roomID);
        }
    }

    public void Exitroom()
    {
        if (IsHostPlayer())
        {
            if (LocalRoomInfo.ClientPlayerID != "")
            {
                LocalRoomInfo.HostPlayerID = LocalRoomInfo.ClientPlayerID;
                LocalRoomInfo.HostPlayerName = LocalRoomInfo.ClientPlayerName;
                LocalRoomInfo.HostPlayerNetID = LocalRoomInfo.ClientPlayerNetID;
                LocalRoomInfo.ClientPlayerNetID = 0;
                LocalRoomInfo.ClientPlayerID = "";
                LocalRoomInfo.ClientPlayerName = "";

                WndManager.Instance.GetWnd<RoomListWnd>().UpdateRoomCell(LocalRoomInfo);
                ExitRoomRpc(LocalRoomInfo);
            }
            else
            {
                LocalRoomInfo.HostPlayerID = "";
                LocalRoomInfo.HostPlayerNetID = 0;
                LocalRoomInfo.HostPlayerName = "";
                WndManager.Instance.GetWnd<RoomListWnd>().RemoveRoomCell(LocalRoomInfo.RoomID);
                RemoveRoomRPC(LocalRoomInfo.RoomID);
            }
        }
        else
        {
            LocalRoomInfo.ClientPlayerID = "";
            LocalRoomInfo.ClientPlayerNetID = 0;
            LocalRoomInfo.ClientPlayerName = "";
            ExitRoomRpc(LocalRoomInfo);
        }
        
        LocalPlayerInfo.RoomID = "";
    }

    [Rpc(SendTo.NotMe)]
    void RemoveRoomRPC(string roomID)
    {
        if (IsServer)
        {
            if (_roomInfoDict.TryGetValue(roomID, out RoomInfo roomInfo))
            {
                // 清理房主的 RoomID
                if (!string.IsNullOrEmpty(roomInfo.HostPlayerID) && 
                    _playerInfoDict.TryGetValue(roomInfo.HostPlayerID, out PlayerInfo hostInfo))
                {
                    hostInfo.RoomID = "";
                    _playerInfoDict[roomInfo.HostPlayerID] = hostInfo;
                }

                // 清理客机的 RoomID
                if (!string.IsNullOrEmpty(roomInfo.ClientPlayerID) && 
                    _playerInfoDict.TryGetValue(roomInfo.ClientPlayerID, out PlayerInfo clientInfo))
                {
                    clientInfo.RoomID = "";
                    _playerInfoDict[roomInfo.ClientPlayerID] = clientInfo;
                }
                _roomInfoDict.Remove(roomID);
            }
        }
        else
        {
            var wnd = WndManager.Instance.GetWnd<RoomListWnd>();
            if (wnd != null)
            {
                wnd.RemoveRoomCell(roomID);
            }
        }
    }

    [Rpc(SendTo.NotMe)]
    void ExitRoomRpc(RoomInfo roomInfo, RpcParams rpcParams = default)
    {
        if (IsServer)
        {
            _roomInfoDict[roomInfo.RoomID] = roomInfo;
            if (rpcParams.Receive.SenderClientId == roomInfo.HostPlayerNetID)
            {
                // 退出的是房主
                if (_playerInfoDict.TryGetValue(roomInfo.HostPlayerID, out PlayerInfo hostInfo))
                {
                    hostInfo.RoomID = "";
                    _playerInfoDict[roomInfo.HostPlayerID] = hostInfo;
                }
            }
            else if (rpcParams.Receive.SenderClientId == roomInfo.ClientPlayerNetID)
            {
                // 退出的是客机
                if (_playerInfoDict.TryGetValue(roomInfo.ClientPlayerID, out PlayerInfo clientInfo))
                {
                    clientInfo.RoomID = "";
                    _playerInfoDict[roomInfo.ClientPlayerID] = clientInfo;
                }
            }
        }
        else
        {
            // 安全检查
            if (LocalRoomInfo.RoomID == roomInfo.RoomID)
            {
                LocalRoomInfo = roomInfo;
                var roomWnd = WndManager.Instance?.GetWnd<RoomWnd>();
                roomWnd?.UpdateRoomInfo(roomInfo, LocalPlayerInfo);
                if (this.GetSystem<ITurnSystem>().State.Value != TurnState.NoMatch)
                {
                    WndManager.Instance.OpenWnd<WinWnd>();
                    WndManager.Instance.GetWnd<WinWnd>().SetFinishTip(true);
                }
            }
            else
            {
                var roomListWnd = WndManager.Instance?.GetWnd<RoomListWnd>();
                roomListWnd?.UpdateRoomCell(roomInfo);
            }
        }
    }


    #region 创建房间相关

    /// <summary>
    ///  主机创建房间
    /// </summary>
    public void CreateRoom()
    {
        //初始化房间信息，此时客机信息为空
        LocalRoomInfo = new RoomInfo()
        {
            HostPlayerNetID = LocalPlayerInfo.PlayerNetID,
            HostPlayerID = LocalPlayerInfo.PlayerID,
            HostPlayerName = LocalPlayerInfo.PlayerName,

            RoomID = Guid.NewGuid().ToString(),

            ClientPlayerID = "",
            ClientPlayerName = "",

            SlotCount = this.GetModel<IRoomModel>().SlotCount.Value,
            HealthCount = this.GetModel<IRoomModel>().HealthCount.Value,
        };
        //将房间与玩家绑定
        LocalPlayerInfo.RoomID = LocalRoomInfo.RoomID;
        UpdatePlayerInfoRpc(LocalPlayerInfo);
        CreateRoomRpc(LocalRoomInfo);

        //更新UI
        WndManager.Instance.GetWnd<RoomListWnd>().AddRoomCell(LocalRoomInfo);
        WndManager.Instance.CloseWnd<RoomListWnd>();
        WndManager.Instance.OpenWnd<RoomWnd>();
        WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(LocalRoomInfo, LocalPlayerInfo);
    }

    /// <summary>
    ///   已经打开RoomList的玩家同步列表
    /// </summary>
    [Rpc(SendTo.NotMe)]
    void CreateRoomRpc(RoomInfo roomInfo)
    {
        if (IsServer)
        {
            _roomInfoDict.Add(roomInfo.RoomID, roomInfo);
        }
        else
        {
            var wnd = WndManager.Instance.GetWnd<RoomListWnd>();
            if (wnd != null)
            {
                wnd.AddRoomCell(roomInfo);
            }
        }
    }

    #endregion

    #region 同步房间列表

    /// <summary>
    /// 第一次打开RoomList的玩家同步列表
    /// </summary>
    public void SyncRoomList()
    {
        SyncRoomListRpc(LocalPlayerInfo);
    }

    [Rpc(SendTo.Server)]
    void SyncRoomListRpc(PlayerInfo playerInfo)
    {
        BaseRpcTarget baseRpcTarget = RpcTarget.Single(playerInfo.PlayerNetID, RpcTargetUse.Temp);

        foreach (var item in _roomInfoDict)
        {
            SyncRoomListLocalRpc(item.Value, baseRpcTarget);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncRoomListLocalRpc(RoomInfo roomInfo, RpcParams rpcParams = default)
    {
        WndManager.Instance.GetWnd<RoomListWnd>().AddRoomCell(roomInfo);
    }

    #endregion

    #region 加入房间相关

    /// <summary>
    ///  加入房间
    /// </summary>
    public void JoinRoom(RoomInfo roomInfo)
    {
        //修改房间信息
        roomInfo.ClientPlayerID = LocalPlayerInfo.PlayerID;
        roomInfo.ClientPlayerName = LocalPlayerInfo.PlayerName;
        roomInfo.ClientPlayerNetID = LocalPlayerInfo.PlayerNetID;
        mRoomModel = this.GetModel<IRoomModel>();
        mRoomModel.SlotCount.Value = roomInfo.SlotCount;
        mRoomModel.HealthCount.Value = roomInfo.HealthCount;
        //将房间与玩家绑定
        LocalPlayerInfo.RoomID = roomInfo.RoomID;
        LocalRoomInfo = roomInfo;
        UpdatePlayerInfoRpc(LocalPlayerInfo);


        //切换视图（匹配模式或房间模式）
        if (roomInfo.isMatch)
        {
            //TODO 匹配模式
            //WndManager.Instance.GetWnd<MatchWnd>().UpdateMatchInfo(roomInfo, LocalPlayerInfo);
        }
        else
        {
            WndManager.Instance.CloseWnd<RoomListWnd>();
            WndManager.Instance.OpenWnd<RoomWnd>();
            WndManager.Instance.GetWnd<RoomWnd>().ReDrawAnim();
            WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(LocalRoomInfo, LocalPlayerInfo);
        }

        JoinRoomRpc(roomInfo);
    }

    [Rpc(SendTo.NotMe)]
    void JoinRoomRpc(RoomInfo roomInfo)
    {
        if (IsServer)
        {
            if (roomInfo.isMatch)
            {
            }
            else
            {
                _roomInfoDict[roomInfo.RoomID] = roomInfo;
            }
        }
        else
        {
            //切换视图（匹配模式或房间模式）
            if (roomInfo.isMatch)
            {
                //TODO 匹配模式
                //WndManager.Instance.GetWnd<MatchWnd>().UpdateMatchInfo(roomInfo, LocalPlayerInfo);
            }
            else
            {
                //房主更新信息
                if (LocalRoomInfo.RoomID == roomInfo.RoomID)
                {
                    LocalRoomInfo = roomInfo;
                    WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomInfo(roomInfo, LocalPlayerInfo);
                }
                //其他玩家更新房间列表
                else
                {
                    WndManager.Instance.GetWnd<RoomListWnd>().UpdateRoomCell(roomInfo);
                }
            }
        }
    }

    #endregion

    #region 房间状态相关：准备

    public void SyncRoomState(bool ready)
    {
        SyncRoomStateRpc(LocalRoomInfo, ready);
    }

    [Rpc(SendTo.NotMe)]
    void SyncRoomStateRpc(RoomInfo roomInfo, bool ready)
    {
        if (IsClient && roomInfo.RoomID == LocalRoomInfo.RoomID)
        {
            WndManager.Instance.GetWnd<RoomWnd>().UpdateRoomState(ready);
        }
    }

    #endregion

    #region 房间状态相关：游戏开始

    public void SyncGameStart()
    {
        SyncGameStartRpc(LocalRoomInfo);
    }

    [Rpc(SendTo.NotMe)]
    void SyncGameStartRpc(RoomInfo roomInfo)
    {
        if (IsClient && roomInfo.RoomID == LocalRoomInfo.RoomID)
        {
            if (roomInfo.isMatch)
            {
                //WndManager.Instance.DeleteWnd<MatchWnd>();
                WndManager.Instance.OpenWnd<GameWnd>();
            }
            else
            {
                WndManager.Instance.CloseWnd<RoomWnd>();
                WndManager.Instance.OpenWnd<GameWnd>();
            }
        }
        else if (IsServer)
        {
            GameNet.Instance.GenerateFirst(roomInfo);
        }
    }

    #endregion

    #region 匹配房间

    /// <summary>
    ///  开始匹配
    /// </summary>
    public void StartMatch()
    {
        StartMatchRpc(LocalPlayerInfo);
    }

    [Rpc(SendTo.Server)]
    void StartMatchRpc(PlayerInfo playerInfo)
    {
        bool hasRoom = false;
        RoomInfo existRoom = new RoomInfo();
        foreach (var item in _matchInfoDict)
        {
            if (item.Value.ClientPlayerID == "")
            {
                hasRoom = true;
                existRoom = item.Value;
                break;
            }
        }

        if (hasRoom)
        {
            BaseRpcTarget baseRpcTarget = RpcTarget.Single(playerInfo.PlayerNetID, RpcTargetUse.Temp);
            JoinRoomLocalRpc(playerInfo, existRoom, baseRpcTarget);
        }
        else
        {
            //没房间就创建房间
            RoomInfo roomInfo = new RoomInfo()
            {
                RoomID = Guid.NewGuid().ToString(),
                HostPlayerNetID = playerInfo.PlayerNetID,
                HostPlayerName = playerInfo.PlayerName,

                ClientPlayerName = "",
                ClientPlayerID = "",
                isMatch = true,
            };
            playerInfo.RoomID = roomInfo.RoomID;

            _matchInfoDict.Add(roomInfo.RoomID, roomInfo);
            BaseRpcTarget baseRpcTarget = RpcTarget.Single(playerInfo.PlayerNetID, RpcTargetUse.Temp);
            CreateRoomLocalRpc(playerInfo, roomInfo, baseRpcTarget);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void JoinRoomLocalRpc(PlayerInfo playerInfo, RoomInfo roomInfo, RpcParams rpcParams = default)
    {
        JoinRoom(roomInfo);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void CreateRoomLocalRpc(PlayerInfo playerInfo, RoomInfo roomInfo, RpcParams rpcParams = default)
    {
        if (LocalPlayerInfo.PlayerID == playerInfo.PlayerID)
        {
            LocalRoomInfo = roomInfo;
            LocalPlayerInfo = playerInfo;
        }
    }

    #endregion

    #region 小工具

    PlayerInfo GetPlayerInfoByNetID(ulong netID)
    {
        PlayerInfo playerInfo = default;
        foreach (var item in _playerInfoDict)
        {
            if (item.Value.PlayerNetID == netID)
            {
                playerInfo = item.Value;
            }
        }

        return playerInfo;
    }

    public bool IsHostPlayer()
    {
        return LocalPlayerInfo.PlayerID == LocalRoomInfo.HostPlayerID;
    }

    public ulong GetRemotePlayerNetID()
    {
        if (LocalPlayerInfo.PlayerID == LocalRoomInfo.HostPlayerID)
        {
            return LocalRoomInfo.ClientPlayerNetID;
        }
        else
        {
            return LocalRoomInfo.HostPlayerNetID;
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return Inscription.Interface;
    }

    #endregion
}