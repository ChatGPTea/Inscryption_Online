using System;
using FunkyCode.LightSettings;
using UnityEngine;

namespace FunkyCode.LightingSettings
{
    [Serializable]
    public class LightmapPresetList
    {
        public LightmapPreset[] list = new LightmapPreset[1];

        public LightmapPreset this[int i] => list[i];

        public string[] GetLightmapLayers()
        {
            var layers = new string[list.Length];

            for (var i = 0; i < list.Length; i++)
                if (list[i].name.Length > 0)
                    layers[i] = list[i].name;
                else
                    layers[i] = "Preset (Id: " + (i + 1) + ")";

            return layers;
        }
    }

    [Serializable]
    public class LightmapPreset
    {
        public enum HDR
        {
            Off,
            Half,
            Float
        }

        public enum Type
        {
            RGB24,
            R8,
            RHalf,
            Depth8
        }

        public string name = "Default";

        public Type type = Type.RGB24;
        public HDR hdr = HDR.Half;

        public Color darknessColor = new(0, 0, 0, 1);
        public int depth = -100;

        public float resolution = 1f;

        public LightmapLayerList dayLayers = new();
        public LightmapLayerList lightLayers = new();

        public LightmapPreset(int id)
        {
            if (id == 0)
                name = "Default";
            else
                name = "Preset (Id: " + (id + 1) + ")";
        }
    }

    [Serializable]
    public class LightmapLayerList
    {
        public LightmapLayer[] list = new LightmapLayer[1];

        public LightmapLayer this[int i] => list[i];

        public void SetArray(LightmapLayer[] array)
        {
            list = array;
        }

        public LightmapLayer[] Get()
        {
            for (var i = 0; i < list.Length; i++)
                if (list[i] == null)
                    list[i] = new LightmapLayer();

            return list;
        }
    }

    [Serializable]
    public class LightmapLayer
    {
        public int id;
        public LayerType type = LayerType.ShadowsAndMask;
        public LayerSorting sorting = LayerSorting.None;

        public int GetLayerID()
        {
            var layerId = id;

            layerId = layerId < 0 ? -1 : layerId;

            return layerId;
        }
    }
}