using FunkyCode.LightingSettings;
using UnityEngine;

namespace FunkyCode.Rendering.Depth
{
    public static class Main
    {
        private static readonly Pass pass = new();

        public static void Draw(Camera camera, LightmapPreset lightmapPreset)
        {
            var layerSettings = lightmapPreset.dayLayers.Get();

            if (layerSettings == null) return;

            if (layerSettings.Length < 1) return;

            for (var i = 0; i < layerSettings.Length; i++)
            {
                var dayLayer = layerSettings[i];

                if (!pass.Setup(dayLayer, camera)) continue;

                Rendering.Draw(pass);
            }
        }

        public static Color ClearColor(LightmapPreset lightmapPreset)
        {
            var depthFloat = ((float)lightmapPreset.depth + 100) / 255;

            var color = new Color(depthFloat, 0, 0, 0);

            return color;
        }
    }
}