using System;
using FunkyCode.LightSettings;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LayerSetting
    {
        public int layerID;
        public LightLayerType type = LightLayerType.ShadowAndMask;

        public LightLayerSorting sorting = LightLayerSorting.None;
        public LightLayerSortingIgnore sortingIgnore = LightLayerSortingIgnore.None;

        public LightLayerShadowEffect shadowEffect = LightLayerShadowEffect.LegacyCPU;
        public int shadowEffectLayer;

        public LightLayerMaskLit maskLit = LightLayerMaskLit.AlwaysLit;
        public float maskLitDistance = 1;

        public int GetLayerID()
        {
            var layer = layerID;

            if (layer < 0) return -1;

            return layer;
        }
    }

    public class DayMaskColor
    {
        public static Color Get(DayLightCollider2D id)
        {
            if (id.maskLit == DayLightCollider2D.MaskLit.LitAbove) return LitAbove();

            return new Color(1, 1, 1, 1);
        }

        public static Color LitAbove()
        {
            var q = Mathf.Cos((Lighting2D.DayLightingSettings.direction + 90) * Mathf.Deg2Rad) * 2;

            q = Mathf.Min(q, 1);
            q = Mathf.Max(q, 0);

            return new Color(q, q, q, 1);
        }
    }

    public class LayerSettingColor
    {
        public static Color Get(LightColliderShape lightShape, Vector2 position, LayerSetting layerSetting,
            MaskLit maskLit, float maskTranslucency, float maskLitCustom)
        {
            switch (maskLit)
            {
                case MaskLit.Unlit:

                    return Color.black;

                case MaskLit.Isometric:

                    var rect = lightShape.GetIsoWorldRect();

                    if (rect.width < rect.height)
                    {
                        var x = position.y + position.x / 2;

                        return LitAbove(x, layerSetting);
                    }

                    var y = position.y - position.x / 2;

                    return LitAbove(y, layerSetting);

                case MaskLit.Custom:

                    return new Color(maskLitCustom, maskLitCustom, maskLitCustom, 1);

                case MaskLit.LitAbove:

                    return LitAbove(position.y, layerSetting);
            }

            switch (layerSetting.maskLit)
            {
                case LightLayerMaskLit.AboveLit:

                    return LitAbove(position.y, layerSetting);

                case LightLayerMaskLit.NeverLit:

                    return Color.black;

                default:

                    return new Color(1, 1, 1, maskTranslucency);
            }
        }

        // Light Tilemap
        public static Color Get(Vector2 position, LayerSetting layerSetting, MaskLit maskLit, float maskTranslucency,
            float maskLitCustom)
        {
            if (maskLit == MaskLit.Unlit) return Color.black;

            if (maskLit == MaskLit.Custom) return new Color(maskLitCustom, maskLitCustom, maskLitCustom, 1);

            if (layerSetting.maskLit == LightLayerMaskLit.AboveLit) return LitAbove(position.y, layerSetting);

            if (layerSetting.maskLit == LightLayerMaskLit.NeverLit) return Color.black;

            return new Color(1, 1, 1, maskTranslucency);
        }

        public static Color LitAbove(float positionDistance, LayerSetting layerSetting)
        {
            var effectDistance = layerSetting.maskLitDistance;
            var q = (positionDistance + effectDistance / 1.5f) / effectDistance;

            q = Mathf.Min(q, 1);
            q = Mathf.Max(q, 0);

            return new Color(q, q, q, 1);
        }
    }
}