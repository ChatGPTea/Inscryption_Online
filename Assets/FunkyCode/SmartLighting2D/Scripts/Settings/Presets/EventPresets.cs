using System;

namespace FunkyCode.LightingSettings
{
    [Serializable]
    public class EventPresetList
    {
        public EventPreset[] list = new EventPreset[2];

        public string[] GetBufferLayers()
        {
            var layers = new string[list.Length];

            for (var i = 0; i < list.Length; i++)
                if (i == 0)
                    layers[i] = "Disabled";
                else if (list[i].name.Length > 0)
                    layers[i] = list[i].name;
                else
                    layers[i] = "Preset (Id: " + i + ")";

            return layers;
        }

        public EventPreset[] Get()
        {
            for (var i = 0; i < list.Length; i++)
                if (list[i] == null)
                    list[i] = new EventPreset(i);

            return list;
        }
    }

    [Serializable]
    public class EventPreset
    {
        public string name = "Default";

        public EventPresetLayers layerSetting = new();

        public EventPreset(int id)
        {
            if (id == 0)
            {
                name = "Disabled";
            }
            else
            {
                if (id == 1)
                    name = "Default";
                else
                    name = "Preset (Id: " + id + ")";
            }
        }
    }

    [Serializable]
    public class EventPresetLayers
    {
        public LayerEventSetting[] list = new LayerEventSetting[1];

        public void SetArray(LayerEventSetting[] array)
        {
            list = array;
        }

        public LayerEventSetting[] Get()
        {
            for (var i = 0; i < list.Length; i++)
                if (list[i] == null)
                    list[i] = new LayerEventSetting();

            return list;
        }
    }
}

[Serializable]
public class LayerEventSetting
{
    public int layerID;

    public int GetLayerID()
    {
        var layer = layerID;

        if (layer < 0) return -1;

        return layer;
    }
}