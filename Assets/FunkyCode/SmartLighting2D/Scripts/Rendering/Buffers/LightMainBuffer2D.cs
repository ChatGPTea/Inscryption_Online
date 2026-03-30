using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.Rendering;
using UnityEngine;

namespace FunkyCode
{
    public class LightMainBuffer2D
    {
        public enum Type
        {
            RGB24,
            R8,
            RHalf,
            Depth8
        }

        public static List<LightMainBuffer2D> List = new();
        public CameraLightmap cameraLightmap;
        public CameraSettings cameraSettings;
        public HDR hdr;

        private Material material;

        public string name = "Uknown";

        public LightTexture renderTexture;
        public bool sceneView;

        public Type type;

        public bool updateNeeded = false;

        public LightMainBuffer2D(bool sceneView, Type type, HDR hdr, CameraSettings cameraSettings,
            CameraLightmap cameraLightmap)
        {
            this.type = type;
            this.hdr = hdr;

            this.cameraLightmap = cameraLightmap;
            this.cameraSettings = cameraSettings;
            this.sceneView = sceneView;

            List.Add(this);
        }

        public bool IsActive => List.IndexOf(this) > -1;

        public static void Clear()
        {
            foreach (var buffer in new List<LightMainBuffer2D>(List)) buffer.DestroySelf();

            List.Clear();
        }

        public void DestroySelf()
        {
            if (renderTexture != null)
                if (renderTexture.renderTexture)
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(renderTexture.renderTexture);
                    }
                    else
                    {
                        renderTexture.renderTexture.Release();
                        renderTexture.renderTexture.DiscardContents();

                        Object.DestroyImmediate(renderTexture.renderTexture);
                    }
                }

            List.Remove(this);
        }

        public static LightMainBuffer2D Get(bool sceneView, CameraSettings cameraSettings, CameraLightmap lightmap,
            LightmapPreset lightmapPreset)
        {
            var type = (Type)lightmapPreset.type;
            var hdr = (HDR)lightmapPreset.hdr;

            if (cameraSettings.GetCamera() == null) return null;

            foreach (var mainBuffer in List)
                if (mainBuffer.hdr == hdr
                    && mainBuffer.type == type
                    && mainBuffer.sceneView == sceneView
                    && mainBuffer.cameraSettings.GetCamera() == cameraSettings.GetCamera()
                    && mainBuffer.cameraLightmap.presetId == lightmap.presetId)
                    return mainBuffer;

            if (Lighting2D.LightmapPresets.Length <= lightmap.presetId)
            {
                Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

                return null;
            }

            var buffer = new LightMainBuffer2D(sceneView, type, hdr, cameraSettings, lightmap);

            LightMainBuffer.InitializeRenderTexture(buffer);

            return buffer;
        }

        public LightmapPreset GetLightmapPreset()
        {
            if (Lighting2D.LightmapPresets.Length <= cameraLightmap.presetId)
            {
                Debug.LogWarning("Lighting2D: Not enough buffer settings initialized");

                return null;
            }

            return Lighting2D.LightmapPresets[cameraLightmap.presetId];
        }

        public void ClearMaterial()
        {
            material = null;
        }

        public Material GetMaterial()
        {
            if (material == null)
                switch (cameraLightmap.overlayMaterial)
                {
                    case CameraLightmap.OverlayMaterial.Multiply:

                        material = new Material(Shader.Find("Light2D/Internal/Multiply"));
                        break;

                    case CameraLightmap.OverlayMaterial.Additive:

                        material = new Material(
                            Shader.Find("Legacy Shaders/Particles/Additive")); // use light 2D shader?	
                        break;

                    case CameraLightmap.OverlayMaterial.Custom:

                        material = new Material(cameraLightmap.GetMaterial());
                        break;

                    case CameraLightmap.OverlayMaterial.Reference:

                        material = cameraLightmap.customMaterial;
                        break;
                }

            if (material)
            {
                if (renderTexture != null)
                    material.mainTexture = renderTexture.renderTexture;
                else
                    Debug.LogWarning("render texture null");
            }

            return material;
        }

        public void Update()
        {
            LightMainBuffer.Update(this);
        }

        public void Render()
        {
            if (cameraLightmap.rendering == CameraLightmap.Rendering.Disabled) return;

            if (updateNeeded)
            {
                var camera = Camera.current;
                if (camera)
                {
                    // return;	
                }

                if (renderTexture != null)
                {
                    var previous = RenderTexture.active;

                    RenderTexture.active = renderTexture.renderTexture;

                    LightMainBuffer.Render(this);

                    RenderTexture.active = previous;
                }
                else
                {
                    Debug.LogWarning(
                        $"null render texture in buffer {cameraSettings.id}:{cameraLightmap.presetId}:{sceneView}");
                }
            }

            LightMainBuffer.DrawOn(this);
        }
    }
}