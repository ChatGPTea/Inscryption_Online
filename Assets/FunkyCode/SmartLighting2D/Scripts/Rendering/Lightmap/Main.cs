using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEngine;
using Texture = FunkyCode.Rendering.Universal.Texture;

namespace FunkyCode.Rendering.Lightmap
{
    public static class Main
    {
        private static readonly Pass pass = new();

        public static void Draw(Camera camera, LightmapPreset lightmapPreset)
        {
            if (Day.Main.IsDrawing(camera, lightmapPreset)) DarknessColor(camera, lightmapPreset);

            var layerSettings = lightmapPreset.lightLayers.Get();
            if (layerSettings == null)
                return;

            if (layerSettings.Length < 1)
                return;

            for (var i = 0; i < layerSettings.Length; i++)
            {
                var lightingLayer = layerSettings[i];
                if (!pass.Setup(lightingLayer, camera))
                    continue;

                if (lightingLayer.sorting == LayerSorting.None)
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

        private static void DarknessColor(Camera camera, LightmapPreset lightmapPreset)
        {
            var color = lightmapPreset.darknessColor;
            if (color.a > 0)
            {
                var material = Lighting2D.Materials.GetAlphaColor(); // use dedicated shader?
                material.mainTexture = null;

                GLExtended.color = color;

                var cameraRotation = -LightingPosition.GetCameraRotation(camera);
                Vector2 size = LightingRender2D.GetSize(camera);

                Texture.Quad.Draw(material, Vector2.zero, size, cameraRotation, 0);
            }
        }

        public static Color ClearColor(Camera camera, LightmapPreset lightmapPreset)
        {
            if (Day.Main.IsDrawing(camera, lightmapPreset)) return Color.white;

            var color = lightmapPreset.darknessColor;
            var alpha = color.a;

            if (alpha > 0)
            {
                var returnColor = Color.white;

                returnColor.r = alpha * color.r + (1 - alpha) * returnColor.r;
                returnColor.g = alpha * color.g + (1 - alpha) * returnColor.g;
                returnColor.b = alpha * color.b + (1 - alpha) * returnColor.b;

                return returnColor;
            }

            return Color.white;
        }
    }
}