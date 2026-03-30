using System;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LightingCameras
    {
        public static int count;

        [SerializeField] public CameraSettings[] cameraSettings;

        public LightingCameras()
        {
            cameraSettings = new CameraSettings[1];
            cameraSettings[0] = new CameraSettings(0);

            count++;
        }

        public int Length => cameraSettings.Length;

        public CameraSettings Get(int id)
        {
            cameraSettings[id].id = id;

            return cameraSettings[id];
        }

        public void Set(int id, CameraSettings settings)
        {
            cameraSettings[id] = settings;
        }
    }
}