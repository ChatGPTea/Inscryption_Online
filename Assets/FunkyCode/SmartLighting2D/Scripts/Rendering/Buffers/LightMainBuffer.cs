using FunkyCode.LightingSettings;
using FunkyCode.Rendering.Lightmap;
using UnityEngine;

namespace FunkyCode.Rendering
{
    public class LightMainBuffer
    {
        public static void Update(LightMainBuffer2D buffer)
        {
            var lightmapPreset = buffer.GetLightmapPreset();

            if (lightmapPreset == null)
            {
                buffer.DestroySelf();
                return;
            }

            if (!Check.CameraSettings(buffer))
            {
                buffer.DestroySelf();
                return;
            }

            var camera = buffer.cameraSettings.GetCamera();

            if (camera == null) return;

            Check.RenderTexture(buffer);
        }

        public static void DrawPost(LightMainBuffer2D buffer)
        {
            if (buffer.cameraLightmap.overlay != CameraLightmap.Overlay.Enabled) return;

            if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) return;

            LightingRender2D.PostRender(buffer);
        }

        public static void DrawOn(LightMainBuffer2D buffer)
        {
            if (buffer.cameraLightmap.overlay != CameraLightmap.Overlay.Enabled) return;

            switch (Lighting2D.RenderingMode)
            {
                case RenderingMode.OnRender:

                    LightingRender2D.OnRender(buffer);

                    break;

                case RenderingMode.OnPreRender:

                    LightingRender2D.PreRender(buffer);

                    break;
            }
        }

        public static void Render(LightMainBuffer2D buffer)
        {
            var camera = buffer.cameraSettings.GetCamera();

            if (camera == null) return;

            var cameraRotation = LightingPosition.GetCameraRotation(camera);
            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, cameraRotation), Vector3.one);

            var sizeY = camera.orthographicSize;
            var sizeX = sizeY * ((float)camera.pixelWidth / camera.pixelHeight);

            var lightmapPreset = buffer.GetLightmapPreset();

            switch (lightmapPreset.type)
            {
                case LightmapPreset.Type.RGB24:
                case LightmapPreset.Type.R8:
                case LightmapPreset.Type.RHalf:

                    // clear darkness color

                    GL.Clear(false, true, Main.ClearColor(camera, lightmapPreset));

                    GL.LoadPixelMatrix(-sizeX, sizeX, -sizeY, sizeY);
                    GL.MultMatrix(matrix);

                    GL.PushMatrix();

                    Day.Main.Draw(camera, lightmapPreset);

                    Main.Draw(camera, lightmapPreset);

                    GL.PopMatrix();

                    break;

                case LightmapPreset.Type.Depth8:

                    // clear R8 depth

                    GL.Clear(false, true, Depth.Main.ClearColor(lightmapPreset));

                    GL.LoadPixelMatrix(-sizeX, sizeX, -sizeY, sizeY);
                    GL.MultMatrix(matrix);

                    GL.PushMatrix();

                    Depth.Main.Draw(camera, lightmapPreset);

                    GL.PopMatrix();

                    break;
            }
        }

        public static Vector2Int GetScreenResolution(LightMainBuffer2D buffer)
        {
            var lightmapPreset = buffer.GetLightmapPreset();

            if (lightmapPreset == null)
            {
                Debug.Log("lightmap preset null");

                return Vector2Int.zero;
            }

            var camera = buffer.cameraSettings.GetCamera();

            if (camera == null)
            {
                Debug.Log("camera null");

                return Vector2Int.zero;
            }

            var resolution = lightmapPreset.resolution;

            var screenWidth = (int)(camera.pixelRect.width * resolution);
            var screenHeight = (int)(camera.pixelRect.height * resolution);

            return new Vector2Int(screenWidth, screenHeight);
        }

        public static void InitializeRenderTexture(LightMainBuffer2D buffer)
        {
            var screen = GetScreenResolution(buffer);

            if (screen.x <= 0 || screen.y <= 0) return;

            var idName = "";

            var bufferID = buffer.cameraLightmap.presetId;

            if (bufferID < Lighting2D.LightmapPresets.Length) idName = Lighting2D.LightmapPresets[bufferID].name + ", ";

            var camera = buffer.cameraSettings.GetCamera();

            buffer.name = "Camera Buffer (" + idName + "" + buffer.type + ", Id: " + (bufferID + 1) + ", Camera: " +
                          camera.name + " )";

            var format = RenderTextureFormat.Default;

            switch (buffer.type)
            {
                case LightMainBuffer2D.Type.RGB24:

                    switch (buffer.hdr)
                    {
                        case HDR.Half:

                            format = RenderTextureFormat.RGB111110Float;

                            break;

                        case HDR.Float:

                            format = RenderTextureFormat.DefaultHDR;

                            break;

                        case HDR.Off:

                            format = RenderTextureFormat.RGB565;

                            break;
                    }

                    break;

                case LightMainBuffer2D.Type.Depth8:

                    format = RenderTextureFormat.R8; // no HDR

                    break;

                case LightMainBuffer2D.Type.R8:

                    format = RenderTextureFormat.R8; // no HDR

                    break;

                case LightMainBuffer2D.Type.RHalf:

                    format = RenderTextureFormat.RHalf; // no HDR

                    break;
            }

            if (!SystemInfo.SupportsRenderTextureFormat(format)) format = RenderTextureFormat.Default;

            buffer.renderTexture = new LightTexture(screen.x, screen.y, 0, format);
            buffer.renderTexture.renderTexture.filterMode = Lighting2D.Profile.qualitySettings.lightmapFilterMode;
            buffer.renderTexture.Create();
        }

        public class Check
        {
            public static void RenderTexture(LightMainBuffer2D buffer)
            {
                var screen = GetScreenResolution(buffer);

                if (screen.x <= 0 && screen.y <= 0) return;

                var camera = buffer.cameraSettings.GetCamera();

                if (buffer.renderTexture == null ||
                    (screen.x == buffer.renderTexture.width && screen.y == buffer.renderTexture.height)) return;

                switch (camera.cameraType)
                {
                    case CameraType.Game:

                        InitializeRenderTexture(buffer);

                        break;

                    case CameraType.SceneView:

                        // scene view pixel rect is constantly changing ( unity bug? )

                        var differenceX = Mathf.Abs(screen.x - buffer.renderTexture.width);
                        var differenceY = Mathf.Abs(screen.y - buffer.renderTexture.height);

                        if (differenceX > 5 || differenceY > 5) InitializeRenderTexture(buffer);

                        break;
                }
            }

            public static bool CameraSettings(LightMainBuffer2D buffer)
            {
                var manager = LightingManager2D.Get();

                var settingsID = buffer.cameraSettings.id;

                if (settingsID >= manager.cameras.Length) return false;

                var cameraSetting = manager.cameras.Get(settingsID);

                var bufferId = buffer.cameraLightmap.id;

                if (bufferId >= cameraSetting.Lightmaps.Length) return false;

                var cameraLightmap = cameraSetting.GetLightmap(bufferId);

                if (cameraLightmap.presetId != buffer.cameraLightmap.presetId) return false;

                switch (buffer.cameraLightmap.sceneView)
                {
                    case CameraLightmap.SceneView.Enabled:

                        if (cameraLightmap.sceneView == CameraLightmap.SceneView.Disabled) return false;

                        break;

                    case CameraLightmap.SceneView.Disabled:

                        if (cameraSetting.cameraType != buffer.cameraSettings.cameraType) return false;

                        break;
                }

                return true;
            }
        }
    }
}