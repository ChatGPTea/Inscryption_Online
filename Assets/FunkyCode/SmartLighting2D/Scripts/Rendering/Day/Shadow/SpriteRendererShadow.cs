using FunkyCode.Utilities;
using UnityEngine;
using Sprite = FunkyCode.Rendering.Universal.Sprite;

namespace FunkyCode.Rendering.Day
{
    public static class SpriteRendererShadow
    {
        private static readonly VirtualSpriteRenderer virtualSpriteRenderer = new();

        public static Texture2D currentTexture;
        public static Material material;

        public static Vector2 cameraOffset;
        public static float direction;
        public static float shadowDistance;

        public static Pair2 pair = Pair2.Zero();

        public static void Begin(Vector2 offset)
        {
            material = Lighting2D.Materials.shadow.GetSpriteShadow();
            material.SetColor("_Darkness", Lighting2D.DayLightingSettings.ShadowColor);

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

            GLExtended.color = new Color(0, 0, 0, 1 - id.shadowTranslucency);

            Sprite.Pass.Draw(id.spriteMeshObject, virtualSpriteRenderer, position, scale,
                id.transform.rotation.eulerAngles.z);
        }

        public static void DrawProjection(DayLightCollider2D id)
        {
            if (!id.InAnyCamera()) return;

            var pos = new Vector2(id.transform.position.x + cameraOffset.x, id.transform.position.y + cameraOffset.y);
            var scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

            var shape = id.mainShape;

            var spriteRenderer = shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null) return;

            virtualSpriteRenderer.sprite = spriteRenderer.sprite;
            virtualSpriteRenderer.flipX = spriteRenderer.flipX;
            virtualSpriteRenderer.flipY = spriteRenderer.flipY;

            var sprite = virtualSpriteRenderer.sprite;

            if (sprite == null) return;

            var texture = sprite.texture;

            if (texture == null) return;

            if (currentTexture != texture)
            {
                if (currentTexture != null) GL.End();

                currentTexture = texture;
                material.mainTexture = currentTexture;

                material.SetPass(0);

                GL.Begin(GL.QUADS);
            }

            var spriteTransform =
                new SpriteTransform(virtualSpriteRenderer, pos, scale, id.transform.rotation.eulerAngles.z);

            var uv = spriteTransform.uv;

            var pivotY = sprite.pivot.y / sprite.texture.height;
            pivotY = uv.y + pivotY;

            var pivotX = sprite.pivot.x / sprite.texture.width;

            pair.A = Vector2.zero;
            pair.B = Vector2.zero;

            pair.A = pos + pair.A.Push(direction + Mathf.PI / 2, id.shadowThickness);
            pair.B = pos + pair.B.Push(direction - Mathf.PI / 2, id.shadowThickness);

            if (Lighting2D.DayLightingSettings.direction < 180)
            {
                var uvx = uv.x;
                uv.x = uv.width;
                uv.width = uvx;
            }

            var v1 = pair.A;
            var v2 = pair.A;
            var v3 = pair.B;
            var v4 = pair.B;

            v2 = v2.Push(direction, id.shadowDistance * shadowDistance);
            v3 = v3.Push(direction, id.shadowDistance * shadowDistance);

            GL.Color(new Color(0, 0, 0, 1 - id.shadowTranslucency));

            GL.TexCoord3(uv.x, pivotY, 0);
            GL.Vertex3(v1.x, v1.y, 0);

            GL.TexCoord3(uv.x, uv.height, 0);
            GL.Vertex3(v2.x, v2.y, 0);

            GL.TexCoord3(uv.width, uv.height, 0);
            GL.Vertex3(v3.x, v3.y, 0);

            GL.TexCoord3(uv.width, pivotY, 0);
            GL.Vertex3(v4.x, v4.y, 0);
        }

        public static void DrawProjectionShape(DayLightCollider2D id)
        {
            if (!id.InAnyCamera()) return;

            var scale = new Vector2(id.transform.lossyScale.x, id.transform.lossyScale.y);

            var shape = id.mainShape;

            var spriteRenderer = shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null) return;

            virtualSpriteRenderer.sprite = spriteRenderer.sprite;
            virtualSpriteRenderer.flipX = spriteRenderer.flipX;
            virtualSpriteRenderer.flipY = spriteRenderer.flipY;

            var sprite = virtualSpriteRenderer.sprite;

            if (sprite == null) return;

            var texture = sprite.texture;

            if (texture == null) return;

            if (currentTexture != texture)
            {
                if (currentTexture != null) GL.End();

                currentTexture = texture;
                material.mainTexture = currentTexture;

                material.SetPass(0);
                GL.Begin(GL.QUADS);
            }

            var polygons = shape.GetPolygonsWorld();

            if (polygons.Count < 1) return;

            var polygon = polygons[0];

            var spriteTransform = new SpriteTransform(virtualSpriteRenderer, Vector2.zero, scale,
                id.transform.rotation.eulerAngles.z);

            var uv = spriteTransform.uv;

            var pivotY = sprite.pivot.y / sprite.texture.height;
            pivotY = uv.y + pivotY;

            var pivotX = sprite.pivot.x / sprite.texture.width;

            pair = Polygon2Helper.GetAxis(polygon, direction);
            pair.A += cameraOffset;
            pair.B += cameraOffset;

            if (Lighting2D.DayLightingSettings.direction > 180)
            {
                var uvx = uv.x;
                uv.x = uv.width;
                uv.width = uvx;
            }

            if (virtualSpriteRenderer.flipX)
            {
                var uvx = uv.x;
                uv.x = uv.width;
                uv.width = uvx;
            }

            var v1 = pair.A;
            var v2 = pair.A;
            var v3 = pair.B;
            var v4 = pair.B;

            v2 = v2.Push(direction, id.shadowDistance * shadowDistance);
            v3 = v3.Push(direction, id.shadowDistance * shadowDistance);

            GL.Color(new Color(0, 0, 0, 1 - id.shadowTranslucency));

            GL.TexCoord3(uv.x, pivotY, 0);
            GL.Vertex3(v1.x, v1.y, 0);

            GL.TexCoord3(uv.x, uv.height, 0);
            GL.Vertex3(v2.x, v2.y, 0);

            GL.TexCoord3(uv.width, uv.height, 0);
            GL.Vertex3(v3.x, v3.y, 0);

            GL.TexCoord3(uv.width, pivotY, 0);
            GL.Vertex3(v4.x, v4.y, 0);
        }
    }
}