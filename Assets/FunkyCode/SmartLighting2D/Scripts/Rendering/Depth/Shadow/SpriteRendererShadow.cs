using FunkyCode.Utilities;
using UnityEngine;
using Sprite = FunkyCode.Rendering.Universal.Sprite;

namespace FunkyCode.Rendering.Depth
{
    public static class SpriteRendererShadow
    {
        private static readonly VirtualSpriteRenderer virtualSpriteRenderer = new();

        public static Texture2D currentTexture;
        public static Material material;

        public static Vector2 cameraOffset;
        public static float direction;
        public static float shadowDistance;

        public static void Begin(Vector2 offset)
        {
            material = Lighting2D.Materials.shadow.GetDepthDayShadow();
            material.mainTexture = null;

            currentTexture = null;

            cameraOffset = offset;
            direction = -Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;
            shadowDistance = Lighting2D.DayLightingSettings.height;
        }

        public static void End()
        {
            GL.End();

            material.mainTexture = null;
            currentTexture = null;
        }

        public static void DrawOffset(DayLightCollider2D id)
        {
            if (!id.InAnyCamera()) return;

            var scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

            var shape = id.mainShape;

            var spriteRenderer = shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null) return;

            virtualSpriteRenderer.sprite = spriteRenderer.sprite;
            virtualSpriteRenderer.flipX = spriteRenderer.flipX;
            virtualSpriteRenderer.flipY = spriteRenderer.flipY;

            if (virtualSpriteRenderer.sprite == null) return;

            var texture = virtualSpriteRenderer.sprite.texture;

            if (texture == null) return;

            if (currentTexture != texture)
            {
                if (currentTexture != null) GL.End();

                currentTexture = texture;
                material.mainTexture = currentTexture;

                material.SetPass(0);
                GL.Begin(GL.QUADS);
            }

            var position = new Vector2(id.transform.position.x + cameraOffset.x,
                id.transform.position.y + cameraOffset.y);
            position.x += Mathf.Cos(direction) * id.mainShape.height * shadowDistance;
            position.y += Mathf.Sin(direction) * id.mainShape.height * shadowDistance;

            var depth = (100f + id.GetDepth()) / 255;

            GLExtended.color = new Color(depth, 0, 0, 1 - id.shadowTranslucency);

            Sprite.Pass.Draw(id.spriteMeshObject, virtualSpriteRenderer, position, scale,
                id.transform.rotation.eulerAngles.z);
        }
    }
}