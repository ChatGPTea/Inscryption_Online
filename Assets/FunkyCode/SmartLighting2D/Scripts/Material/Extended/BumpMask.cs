using System;
using UnityEngine;

namespace FunkyCode.Lighting2DMaterial
{
    [Serializable]
    public class BumpMask
    {
        private LightingMaterial bumpedDaySprite;
        private LightingMaterial normalObjectToLightSprite;

        private LightingMaterial normalPixelToLightSprite;


        public void Reset()
        {
            normalObjectToLightSprite = null;
            normalPixelToLightSprite = null;
            bumpedDaySprite = null;
        }

        public void Initialize()
        {
            GetNormalMapSpritePixelToLight();
            GetNormalMapSpriteObjectToLight();

            GetBumpedDaySprite();
        }


        public Material GetNormalMapSpritePixelToLight()
        {
            if (normalPixelToLightSprite == null || normalPixelToLightSprite.Get() == null)
                normalPixelToLightSprite = LightingMaterial.Load("Light2D/Internal/BumpMap/PixelToLight");
            return normalPixelToLightSprite.Get();
        }

        public Material GetNormalMapSpriteObjectToLight()
        {
            if (normalObjectToLightSprite == null || normalObjectToLightSprite.Get() == null)
                normalObjectToLightSprite = LightingMaterial.Load("Light2D/Internal/BumpMap/ObjectToLight");
            return normalObjectToLightSprite.Get();
        }

        public Material GetBumpedDaySprite()
        {
            if (bumpedDaySprite == null || bumpedDaySprite.Get() == null)
                bumpedDaySprite = LightingMaterial.Load("Light2D/Internal/BumpMap/Day");
            return bumpedDaySprite.Get();
        }
    }
}