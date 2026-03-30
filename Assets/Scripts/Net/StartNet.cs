using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

// ========== 添加断开连接处理 ==========
public class StartNet : NetworkBehaviour
{
    public static StartNet Instance;
    
    // 记录已连接的客户端
    private HashSet<ulong> _connectedClients = new HashSet<ulong>();

    private void Start()
    {
        Instance = this;
    }

    public void StartServer(ushort port)
    {
        UnityTransport unityTransport = NetworkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData("0.0.0.0", port);
        NetworkManager.StartServer();
    }

    public void StartClient(string ip, ushort port)
    {
        UnityTransport unityTransport = NetworkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, port);
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallBack;
        NetworkManager.StartClient();
    }
    
    private void NetworkManager_OnClientConnectedCallBack(ulong obj)
    {
        WndManager.Instance.CloseWnd<StartWnd>();
        WndManager.Instance.CloseWnd<LoginWnd>();
        WndManager.Instance.OpenWnd<LobbyWnd>();
    }
}