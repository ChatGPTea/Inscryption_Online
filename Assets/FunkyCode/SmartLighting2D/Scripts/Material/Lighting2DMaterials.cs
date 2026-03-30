using System;
using FunkyCode.Lighting2DMaterial;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class Lighting2DMaterials
    {
        public Mask mask = new();
        public BumpMask bumpMask = new();
        public Shadow shadow = new();
        public Room room = new();
        public Lights lights = new();

        private LightingMaterial additive;
        private LightingMaterial alphablend;

        private bool initialized;
        private LightingMaterial lightSprite;
        private LightingMaterial maskBlurHorizontal;

        private LightingMaterial maskBlurVertical;

        private LightingMaterial multiplyHDR;
        private LightingMaterial occlusionBlur;
        private LightingMaterial occlusionEdge;

        public bool Initialize()
        {
            if (initialized) return false;

            Reset();

            mask.Reset();
            shadow.Reset();
            room.Reset();
            bumpMask.Reset();
            lights.Reset();

            initialized = true;

            mask.Initialize();
            shadow.Initialize();
            room.Initialize();
            bumpMask.Initialize();
            lights.Initialize();

            GetAdditive();

            GetOcclusionBlur();
            GetOcclusionEdge();

            return true;
        }

        public void Reset()
        {
            // is it the best way?
            initialized = false;

            maskBlurVertical = null;
            maskBlurHorizontal = null;

            occlusionEdge = null;
            occlusionBlur = null;

            additive = null;
            multiplyHDR = null;
            alphablend = null;

            lightSprite = null;
        }

        public Material GetLightSprite()
        {
            if (lightSprite == null || lightSprite.Get() == null)
                lightSprite = LightingMaterial.Load("Light2D/Internal/LightSprite/Light");

            return lightSprite.Get();
        }

        public Material GetMaskBlurVertical()
        {
            if (maskBlurVertical == null || maskBlurVertical.Get() == null)
                maskBlurVertical = LightingMaterial.Load("Light2D/Internal/BlurVertical");

            return maskBlurVertical.Get();
        }

        public Material GetMaskBlurHorizontal()
        {
            if (maskBlurHorizontal == null || maskBlurHorizontal.Get() == null)
                maskBlurHorizontal = LightingMaterial.Load("Light2D/Internal/BlurHorizontal");

            return maskBlurHorizontal.Get();
        }

        public Material GetAdditive()
        {
            if (additive == null || additive.Get() == null)
                additive = LightingMaterial.Load("Light2D/Internal/Additive");

            return additive.Get();
        }

        public Material GetMultiplyHDR()
        {
            if (multiplyHDR == null || multiplyHDR.Get() == null)
                multiplyHDR = LightingMaterial.Load("Light2D/Internal/Multiply HDR");

            return multiplyHDR.Get();
        }

        public Material GetAlphaColor()
        {
            if (alphablend == null || alphablend.Get() == null)
                alphablend = LightingMaterial.Load("Light2D/Internal/AlphaColor");

            return alphablend.Get();
        }

        public Material GetOcclusionEdge()
        {
            if (occlusionEdge == null || occlusionEdge.Get() == null)
            {
                occlusionEdge = LightingMaterial.Load("Light2D/Internal/Multiply HDR");

                occlusionEdge.SetTexture("textures/occlusionedge");
            }

            return occlusionEdge.Get();
        }

        public Material GetOcclusionBlur()
        {
            if (occlusionBlur == null || occlusionBlur.Get() == null)
            {
                occlusionBlur = LightingMaterial.Load("Light2D/Internal/Multiply HDR");

                occlusionBlur.SetTexture("textures/occlussionblur");
            }

            return occlusionBlur.Get();
        }
    }
}