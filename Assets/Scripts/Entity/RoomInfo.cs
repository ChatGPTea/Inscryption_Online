using Unity.Netcode;

public struct RoomInfo : INetworkSerializable
{
    public string RoomID;
    public bool isMatch;
    public int SlotCount;
    public int HealthCount;
    
    public ulong HostPlayerNetID;
    public string HostPlayerID;
    public string HostPlayerName;
    
    public ulong ClientPlayerNetID;
    public string ClientPlayerID;
    public string ClientPlayerName;
    
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref RoomID);
        serializer.SerializeValue(ref isMatch);
        serializer.SerializeValue(ref SlotCount);
        serializer.SerializeValue(ref HealthCount);
        
        serializer.SerializeValue(ref HostPlayerNetID);
        serializer.SerializeValue(ref HostPlayerID);
        serializer.SerializeValue(ref HostPlayerName);
        
        serializer.SerializeValue(ref ClientPlayerID);
        serializer.SerializeValue(ref ClientPlayerNetID);
        serializer.SerializeValue(ref ClientPlayerName);
    }
}