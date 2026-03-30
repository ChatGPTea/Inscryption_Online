using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCell : MonoBehaviour
{
    public RoomInfo RoomInfo;
    
    public bool canJoin = false;

    public void Initial(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        TMP_Text roomNameText = GetComponentInChildren<TMP_Text>();
        roomNameText.text = $"F:/Create/Rooms/<color=#D7E2A3>{roomInfo.HostPlayerName}<color=#9C2431> 创建的房间";
        UpdateCell(roomInfo);
    }
    
    public void UpdateCell(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        TMP_Text roomNameText = GetComponentInChildren<TMP_Text>();
        roomNameText.text = $"F:/Create/Rooms/<color=#D7E2A3>{roomInfo.HostPlayerName}<color=#9C2431> 创建的房间";
        if (RoomInfo.HostPlayerID != "" && RoomInfo.ClientPlayerID != "")
        {
            canJoin = false;
        }
        else
        {
            canJoin = true;
        }
    }

    public void OnClickBtn()
    {
        WndManager.Instance.GetWnd<RoomListWnd>().UpdateRoomInfo(RoomInfo);
    }
}