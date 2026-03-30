using System;
using Inscryption;
using QFramework;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class GameNet : NetworkBehaviour,ICanGetSystem,ICanSendCommand,ICanSendQuery
{
    public static GameNet Instance;
    
    public bool isMultiplayer = false;
    

    private void Awake()
    {
        Instance = this;
    }

    #region 初始化牌桌

    public void GenerateFirst(RoomInfo roomInfo)
    {
        int random = Random.Range(0, 10);
        BaseRpcTarget baseRpcTarget = RpcTarget.Group(new ulong[] { roomInfo.HostPlayerNetID, roomInfo.ClientPlayerNetID }, RpcTargetUse.Temp);
        if (random < 5)
        {
            //房主先手
            GenerateFistRpc(true, baseRpcTarget);
        }
        else
        {
            GenerateFistRpc(false, baseRpcTarget);
        }
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void GenerateFistRpc(bool hostFirst, RpcParams rpcParams = default)
    {
        if (hostFirst)
        {
            if (LobbyNet.Instance.IsHostPlayer())
            {
                WndManager.Instance.GetWnd<GameWnd>().DecideWhoFirst(true);
            }
            else
            {
                WndManager.Instance.GetWnd<GameWnd>().DecideWhoFirst(false);
            }
        }
        else
        {
            if (LobbyNet.Instance.IsHostPlayer())
            {
                WndManager.Instance.GetWnd<GameWnd>().DecideWhoFirst(false);
            }
            else
            {
                WndManager.Instance.GetWnd<GameWnd>().DecideWhoFirst(true);
            }
        }

    }

    #endregion
    
    #region 回合同步
    
    public void SyncTurn()
    {
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncTurnRpc(target);
    } 

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncTurnRpc(RpcParams rpcParams = default)
    {
        this.GetSystem<ITurnSystem>().State.Value = TurnState.EnemyAttack;
    }
    
    #endregion

    #region 手牌同步

    /// <summary>
    ///  抽卡同步
    /// </summary>
    public void SyncDrawCard(int cardID)
    {
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncDrawCardRpc(cardID, target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncDrawCardRpc(int cardID, RpcParams rpcParams = default)
    {
        WndManager.Instance.GetWnd<GameWnd>().CardInHandManager.EnemyDrawCard(cardID);
    }

    /// <summary>
    /// 选卡同步
    /// </summary>
    
    public void SyncSelectCard(int index)
    {
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncSelectCardRpc(index, target);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void SyncSelectCardRpc(int index, RpcParams rpcParams = default)
    {
        this.SendCommand(new EnemyCardSelectCommand(index));
    }
    
    /// <summary>
    /// 放置卡牌同步
    /// </summary>
    public void SyncPlaceCard(int slotIndex)
    {
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncPlaceCardRpc(slotIndex, target);
    }
    
    public struct EnemySlotSelectEvent
    {
        public int slotIndex;
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void SyncPlaceCardRpc(int index, RpcParams rpcParams = default)
    {
        TypeEventSystem.Global.Send(new EnemySlotSelectEvent()
        {
            slotIndex = index,
        });
    }

    /// <summary>
    /// 献祭卡牌同步
    /// </summary>
    public void SyncSacrifice(int slotIndex)
    {
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncSacrificeRpc(slotIndex, target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncSacrificeRpc(int slotIndex, RpcParams rpcParams = default)
    {
        var mCard = this.SendQuery(new FindTargetByIndexQuery(slotIndex, true));
        mCard.GetComponent<CardInSlot>().QuitSacrifice(false);
    }

    #endregion

    #region 结果同步

    public void SyncResult()
    {        
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncResultRpc(target);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void SyncResultRpc(RpcParams rpcParams = default)
    {
        if (IsServer)
        {
            Debug.Log($"[Server]  - 跳过 UI 处理");
            return;
        }
        WndManager.Instance?.OpenWnd<WinWnd>();
        var winWnd = WndManager.Instance?.GetWnd<WinWnd>();
        if (winWnd != null)
        {
            winWnd.SetFinishTip(false);
        }
    }    
    
    
    public void SyncWinResult()
    {        
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncWinResultRpc(target);
    }
    
    [Rpc(SendTo.SpecifiedInParams)]
    void SyncWinResultRpc(RpcParams rpcParams = default)
    {
        if (IsServer)
        {
            Debug.Log($"[Server]  - 跳过 UI 处理");
            return;
        }

        if (WndManager.Instance.GetWnd<GameWnd>() == null) return;
        WndManager.Instance?.OpenWnd<WinWnd>();
        var winWnd = WndManager.Instance?.GetWnd<WinWnd>();
        if (winWnd != null)
        {
            winWnd.SetFinishTip(true);
        }
    }

    //返回房间
    public void SyncBack()
    {        
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncBackRpc(target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncBackRpc(RpcParams rpcParams = default)
    {
        var roomWnd = WndManager.Instance?.GetWnd<RoomWnd>();
        if (roomWnd != null)
        {
            roomWnd.mIsReadyToggle.isOn = false;
            roomWnd.OnReadyToggle(false);
        }
    }
    
    #endregion

    #region 对话同步

    public void SyncMessage(string message)
    {        
        if (LobbyNet.Instance.GetRemotePlayerNetID() == 0) return;
        BaseRpcTarget target = RpcTarget.Single(LobbyNet.Instance.GetRemotePlayerNetID(), RpcTargetUse.Temp);
        SyncMessageRpc(message,target);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void SyncMessageRpc(string message,RpcParams rpcParams = default)
    {
        WndManager.Instance.GetWnd<GameWnd>().ActiveEnemyContent(message);
    }
    

    #endregion

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return Inscription.Interface;
    }
}