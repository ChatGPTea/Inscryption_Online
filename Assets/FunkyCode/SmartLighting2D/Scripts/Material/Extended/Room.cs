using System;
using UnityEngine;

namespace FunkyCode.Lighting2DMaterial
{
    [Serializable]
    public class Room
    {
        private LightingMaterial roomMask;
        private LightingMaterial roomMultiply;

        public void Reset()
        {
            roomMask = null;
            roomMultiply = null;
        }

        public void Initialize()
        {
            GetRoomMask();
            GetRoomMultiply();
        }

        public Material GetRoomMask()
        {
            if (roomMask == null || roomMask.Get() == null)
                roomMask = LightingMaterial.Load("Light2D/Internal/RoomMask");
            return roomMask.Get();
        }

        public Material GetRoomMultiply()
        {
            if (roomMultiply == null || roomMultiply.Get() == null)
                roomMultiply = LightingMaterial.Load("Light2D/Internal/RoomMultiply");
            return roomMultiply.Get();
        }
    }
}