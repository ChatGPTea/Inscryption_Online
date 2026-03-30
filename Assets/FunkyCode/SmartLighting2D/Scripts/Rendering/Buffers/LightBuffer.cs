using FunkyCode.Rendering.Light;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering
{
    public class LightBuffer
    {
        public static void Render(Light2D light)
        {
            var size = light.size;

            GL.PushMatrix();

            if (light.IsPixelPerfect())
            {
                var camera = Camera.main;

                var cameraRotation = LightingPosition.GetCameraRotation(camera);
                var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, cameraRotation), Vector3.one);

                var sizeY = camera.orthographicSize;
                var sizeX = sizeY * ((float)camera.pixelWidth / camera.pixelHeight);

                GL.LoadPixelMatrix(-sizeX, sizeX, -sizeY, sizeY);
            }
            else
            {
                GL.LoadPixelMatrix(-size, size, -size, size);
            }

            Main.Draw(light);

            GL.PopMatrix();

            light.drawingEnabled = ShadowEngine.continueDrawing;
        }

        public static void RenderTranslucency(Light2D light)
        {
            var size = light.size;

            GL.PushMatrix();

            if (light.IsPixelPerfect())
            {
                var camera = Camera.main;

                var cameraRotation = LightingPosition.GetCameraRotation(camera);
                var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, cameraRotation), Vector3.one);

                var sizeY = camera.orthographicSize;
                var sizeX = sizeY * ((float)camera.pixelWidth / camera.pixelHeight);

                GL.LoadPixelMatrix(-sizeX, sizeX, -sizeY, sizeY);
            }
            else
            {
                GL.LoadPixelMatrix(-size, size, -size, size);
            }

            Main.DrawTranslucency(light);

            GL.PopMatrix();

            light.drawingTranslucencyEnabled = ShadowEngine.continueDrawing;
        }

        public static void RenderFreeForm(Light2D light)
        {
            var freeForm = light.freeForm;

            var points = freeForm.polygon.points;

            var pointsCount = points.Length;
            if (pointsCount < 3)
                return;

            var size = light.size;
            GL.LoadPixelMatrix(-size, size, -size, size);

            GL.PushMatrix();

            var meshObject = MeshObject.Get(freeForm.polygon.CreateMesh(Vector2.zero, Vector2.zero));

            var material = Lighting2D.Materials.GetAdditive();
            material.mainTexture = null;

            GLExtended.color = Color.white;

            material.SetPass(0);

            GLExtended.DrawMeshPass(meshObject);

            // make new free form edge material (min/max - not additive
            // load new edge texture to the material
            // draw image line function

            if (light.freeFormFalloff > 0)
            {
                var edgeSize = light.freeFormFalloff;

                material = Lighting2D.Materials.lights.GetFreeFormEdgeLight();
                material.mainTexture = null;
                material.SetFloat("_Strength", light.freeFormFalloffStrength);

                material.SetPass(0);

                GL.Begin(GL.QUADS);

                for (var i = 0; i < pointsCount; i++)
                {
                    var point = points[i];
                    var nextPoint = points[(i + 1) % pointsCount];

                    var direction = point.Atan2(nextPoint);

                    var p3 = nextPoint;
                    var p4 = point;

                    var p1 = point;
                    p1.x += Mathf.Cos(direction - Mathf.PI / 2) * edgeSize;
                    p1.y += Mathf.Sin(direction - Mathf.PI / 2) * edgeSize;

                    var p2 = nextPoint;
                    p2.x += Mathf.Cos(direction - Mathf.PI / 2) * edgeSize;
                    p2.y += Mathf.Sin(direction - Mathf.PI / 2) * edgeSize;

                    GL.Color(new Color(0, 0, 0, 0));
                    GL.Vertex3(p1.x, p1.y, 0);
                    GL.Vertex3(p2.x, p2.y, 0);

                    GL.Color(new Color(1, 1, 0, 0));
                    GL.Vertex3(p3.x, p3.y, 0);
                    GL.Vertex3(p4.x, p4.y, 0);

                    GL.Color(new Color(0, 0, 0, 1));
                    GL.Vertex3(point.x - edgeSize, point.y - edgeSize, 0);

                    GL.Color(new Color(1, 0, 0, 1));
                    GL.Vertex3(point.x + edgeSize, point.y - edgeSize, 0);

                    GL.Color(new Color(1, 1, 0, 1));
                    GL.Vertex3(point.x + edgeSize, point.y + edgeSize, 0);

                    GL.Color(new Color(0, 1, 0, 1));
                    GL.Vertex3(point.x - edgeSize, point.y + edgeSize, 0);
                }

                GL.End();
            }

            GL.PopMatrix();
        }

        public static void UpdateName(LightBuffer2D buffer)
        {
            var freeString = string.Empty;

            if (buffer.Free)
                freeString = "free";
            else
                freeString = "taken";

            if (buffer.renderTexture != null)
                buffer.name =
                    $"Buffer (Id: {LightBuffer2D.List.IndexOf(buffer) + 1}, Size: {buffer.renderTexture.width}, {freeString})";
            else
                buffer.name = "Buffer (Id: {LightBuffer2D.List.IndexOf(buffer) + 1}, No Texture, {freeString})";
        }

        public static void InitializeRenderTexture(LightBuffer2D buffer, Vector2Int textureSize)
        {
            var format = RenderTextureFormat.R8;
            if (!SystemInfo.SupportsRenderTextureFormat(format)) format = RenderTextureFormat.Default;

            buffer.renderTexture = new LightTexture(textureSize.x, textureSize.y, 0, format);
            buffer.renderTexture.renderTexture.filterMode = Lighting2D.Profile.qualitySettings.lightFilterMode;

            UpdateName(buffer);
        }

        public static void InitializeFreeFormTexture(LightBuffer2D buffer, Vector2Int textureSize)
        {
            var format = RenderTextureFormat.R8;
            if (!SystemInfo.SupportsRenderTextureFormat(format)) format = RenderTextureFormat.Default;

            buffer.freeFormTexture = new LightTexture(textureSize.x, textureSize.y, 0, format);
            buffer.freeFormTexture.renderTexture.filterMode = Lighting2D.Profile.qualitySettings.lightFilterMode;

            UpdateName(buffer);
        }

        public static void InitializeTranslucencyTexture(LightBuffer2D buffer, Vector2Int textureSize)
        {
            var format = RenderTextureFormat.R8;
            if (!SystemInfo.SupportsRenderTextureFormat(format)) format = RenderTextureFormat.Default;

            buffer.translucencyTexture = new LightTexture(textureSize.x, textureSize.y, 0, format);
            buffer.translucencyTexture.renderTexture.filterMode = Lighting2D.Profile.qualitySettings.lightFilterMode;

            buffer.translucencyTextureBlur = new LightTexture(textureSize.x, textureSize.y, 0, format);
            buffer.translucencyTextureBlur.renderTexture.filterMode =
                Lighting2D.Profile.qualitySettings.lightFilterMode;

            UpdateName(buffer);
        }
    }
}