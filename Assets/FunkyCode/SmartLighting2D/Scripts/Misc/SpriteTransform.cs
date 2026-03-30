using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public struct SpriteTransform
    {
        public Vector2 position;
        public Vector2 scale;
        public float rotation;

        public Rect uv;

        public SpriteTransform(VirtualSpriteRenderer spriteRenderer, Vector2 position, Vector2 scale, float rotation)
        {
            var sprite = spriteRenderer.sprite;

            /*
            if (spriteRenderer == null || sprite == null) {
                this.rotation = 0;
                this.scale = Vector2.zero;
                this.uv = new Rect();
                this.position = Vector2.zero;

                return;
            }*/

            if (!sprite)
            {
                this.position = Vector2.zero;
                this.scale = Vector2.zero;
                this.rotation = 0;
                uv = new Rect(0, 0, 0, 0);
                return;
            }

            var spriteTexture = sprite.texture;

            float textureWidth = spriteTexture.width;
            float textureHeight = spriteTexture.height;

            var spriteRect = sprite.textureRect;

            var spriteWidth = spriteRect.width;
            var spriteHeight = spriteRect.height;

            // Scale
            var textureScale = new Vector2(
                textureWidth / spriteWidth,
                textureHeight / spriteHeight
            );

            var pixelsPerUnit = sprite.pixelsPerUnit * 2;

            scale.x = scale.x / textureScale.x * (textureWidth / pixelsPerUnit);
            scale.y = scale.y / textureScale.y * (textureHeight / pixelsPerUnit);

            if (spriteRenderer.flipX)
                scale.x = -scale.x;

            if (spriteRenderer.flipY)
                scale.y = -scale.y;

            // Pivot
            var pivot = sprite.pivot;

            pivot.x = (pivot.x / spriteWidth - 0.5f) * (scale.x * 2);
            pivot.y = (pivot.y / spriteHeight - 0.5f) * (scale.y * 2);

            // Matrix Projection
            var angle = rotation * Mathf.Deg2Rad + Mathf.PI;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            this.position.x = position.x + pivot.x * cos - pivot.y * sin;
            this.position.y = position.y + pivot.x * sin + pivot.y * cos;

            // UV coordinates
            var uvRect = new Rect();
            uvRect.x = spriteRect.x / textureWidth;
            uvRect.y = spriteRect.y / textureHeight;
            uvRect.width = spriteWidth / textureWidth + uvRect.x;
            uvRect.height = spriteHeight / textureHeight + uvRect.y;

            uv = uvRect;

            this.scale = scale;

            this.rotation = rotation;
        }
    }
}