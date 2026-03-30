using Unity.Netcode;

public struct PlayerInfo : INetworkSerializable
{
    public ulong PlayerNetID;
    public string PlayerID;
    public string PlayerName;
    public string RoomID;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // 防御性检查：确保 string 字段不为 null
        if (PlayerID == null) PlayerID = string.Empty;
        if (PlayerName == null) PlayerName = string.Empty;
        if (RoomID == null) RoomID = string.Empty;
    
        serializer.SerializeValue(ref PlayerNetID);
        serializer.SerializeValue(ref PlayerID);
        serializer.SerializeValue(ref RoomID);
        serializer.SerializeValue(ref PlayerName);
    }
}