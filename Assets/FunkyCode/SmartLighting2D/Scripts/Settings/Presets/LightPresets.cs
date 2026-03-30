using System;

namespace FunkyCode.LightingSettings
{
    [Serializable]
    public class LightPresetList
    {
        public LightPreset[] list = new LightPreset[1];

        public LightPreset this[int i] => list[i];

        public string[] GetPresetNames()
        {
            var layers = new string[list.Length];

            for (var i = 0; i < list.Length; i++)
                if (list[i].name.Length > 0)
                    layers[i] = list[i].name;
                else
                    layers[i] = "Preset (Id: " + (i + 1) + ")";

            return layers;
        }

        public LightPreset[] Get()
        {
            for (var i = 0; i < list.Length; i++)
                if (list[i] == null)
                    list[i] = new LightPreset(i);

            return list;
        }
    }

    [Serializable]
    public class LightPreset
    {
        public string name = "Default";

        public LightPresetLayers layerSetting = new();

        public LightPreset(int id)
        {
            if (id == 0)
                name = "Default";
            else
                name = "Preset (Id: " + (id + 1) + ")";
        }
    }

    [Serializable]
    public class LightPresetLayers
    {
        public LayerSetting[] list = new LayerSetting[1];

        public LayerSetting this[int i] => list[i];

        public void SetArray(LayerSetting[] array)
        {
            list = array;
        }

        public LayerSetting[] Get()
        {
            for (var i = 0; i < list.Length; i++)
                if (list[i] == null)
                    list[i] = new LayerSetting();

            return list;
        }
    }
}