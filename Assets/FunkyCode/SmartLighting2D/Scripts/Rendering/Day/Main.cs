using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEngine;

namespace FunkyCode.Rendering.Day
{
    public static class Main
    {
        private static readonly Pass pass = new();

        public static void Draw(Camera camera, LightmapPreset lightmapPreset)
        {
            if (!IsDrawing(camera, lightmapPreset)) return;

            var layerSettings = lightmapPreset.dayLayers.Get();

            for (var i = 0; i < layerSettings.Length; i++)
            {
                var dayLayer = layerSettings[i];

                var sorting = dayLayer.sorting;

                if (!pass.Setup(dayLayer, camera)) continue;

                if (sorting == LayerSorting.None)
                {
                    NoSort.Draw(pass);
                }
                else
                {
                    pass.SortObjects();

                    Sorted.Draw(pass);
                }
            }
        }

        public static bool IsDrawing(Camera camera, LightmapPreset lightmapPreset)
        {
            if (Lighting2D.DayLightingSettings.ShadowColor.a == 0) // <=
                return false;

            if (lightmapPreset == null) return false;

            var layerSettings = lightmapPreset.dayLayers.Get();

            if (layerSettings.Length < 1) return false;

            return true;
        }
    }
}