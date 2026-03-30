using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public class LightingDebug
    {
        public static float atlasTimer = 0;

        public static TimerHelper timer;

        private static Object[] lights;
        private static Object[] colliders;
        private static Object[] sprites;
        private static Object[] tilemaps;

        public static void OnGUI()
        {
            if (lights == null)
            {
                lights = Object.FindObjectsOfType(typeof(Light2D));
                colliders = Object.FindObjectsOfType(typeof(LightCollider2D));
                sprites = Object.FindObjectsOfType(typeof(LightSprite2D));

                tilemaps = Object.FindObjectsOfType(typeof(LightTilemapCollider2D));
            }

            if (timer == null) timer = TimerHelper.Create();

            if (timer.GetMillisecs() > 1000) SecondUpdate();

            var count = 0;
            foreach (var light in Light2D.List)
            {
                if (!light.InCameras()) continue;

                count++;
            }

            var manager2D = LightingManager2D.Get();
            //LightMainBuffer2D mainBuffer = LightMainBuffer2D.Get();

            GUI.skin.GetStyle("label").alignment = TextAnchor.UpperLeft;

            var textSpace = 15;

            var y = textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Total Custom Physics Shapes: " + totalPhysicsShapes);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Total Custom Physics Shape Meshes: " + totalObjectMaskMeshGenerations);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "Atlas Timer: " + atlasTimer);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Camera Size: " + mainBuffer.cameraSize);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "New Render Textures: " + NewRenderTextures);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "Lights in Camera: " + count + "/" + Light2D.List.Count);

            y += textSpace;

            //  GUI.Label(new Rect(10, y, 500, 20), "Free Buffers: " + LightBuffers.GetFreeCount() + "/" + LightBuffers.GetList().Count);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Light Buffer Updates: " + ShowLightBufferUpdates); // In Frame

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Total Light Updates: " + totalLightUpdates);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Mask Generations: " + show_maskGenerations);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Shadow Generations: " + show_shadowGenerations);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Objects Culled: " + show_culled);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Shadow Collider World Generations C: " + ShadowColliderTotalGenerationsWorld_collider + " (re: " + ShadowColliderTotalGenerationsWorld_collider_re +") Pair: " + LightingDebug.ShadowColliderTotalGenerationsWorld_collider_pair);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Shadow Collider Local Generations C: " + ShadowColliderTotalGenerationsLocal_collider);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Shadow Collider World Generations S: " + ShadowColliderTotalGenerationsWorld_shape + " (re: " + ShadowColliderTotalGenerationsWorld_shape_re +") Pair: " + LightingDebug.ShadowColliderTotalGenerationsWorld_shape_pair);

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Shadow Collider Local Generations S: " + ShadowColliderTotalGenerationsLocal_shape);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Convex Hull Generations: " + ShowConvexHullGenerations);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Sprite Renderers Drawn: " + ShowSpriteRenderersDrawn);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;

            //GUI.Label(new Rect(10, y, 500, 20), "Light Main Buffer Updates: " + ShowLightMainBufferUpdates);

            y += textSpace;

            GUI.Label(new Rect(10, y, 500, 20), "=========================");

            y += textSpace;
            /*
                    Texture texture = LightMainBuffer2D.Get().bufferCamera.activeTexture;
                    if (texture != null) {
                        GUI.Label(new Rect(10, y, 500, 20), "Main Buffer Resolution: " + texture.width + "x" + texture.height);
                    } else {
                        GUI.Label(new Rect(10, y, 500, 20), "Main Buffer Resolution: NULL");
                    }*/

            y += textSpace;


            RightBottomPanel();
        }

        public static void RightBottomPanel()
        {
            var style = new GUIStyle();
            style.alignment = TextAnchor.LowerRight;
            style.normal.textColor = Color.white;
            style.fontSize = 13;

            GUI.Label(new Rect(0, -10, Screen.width - 10, Screen.height), "Tilemap Collider Count: " + tilemaps.Length,
                style);

            GUI.Label(new Rect(0, -30, Screen.width - 10, Screen.height), "Lights Count: " + lights.Length, style);
            GUI.Label(new Rect(0, -50, Screen.width - 10, Screen.height), "Colliders Count: " + colliders.Length,
                style);
            GUI.Label(new Rect(0, -70, Screen.width - 10, Screen.height), "Sprite Renderers Count: " + sprites.Length,
                style);
        }

        public static void SecondUpdate()
        {
            timer = TimerHelper.Create();

            lights = Object.FindObjectsOfType(typeof(Light2D));
            colliders = Object.FindObjectsOfType(typeof(LightCollider2D));
            sprites = Object.FindObjectsOfType(typeof(LightSprite2D));
        }
    }
}