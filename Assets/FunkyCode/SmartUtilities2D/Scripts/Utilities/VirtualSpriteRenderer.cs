using UnityEngine;

namespace FunkyCode.Utilities
{
    public class VirtualSpriteRenderer
    {
        public Color color = Color.white;

        public SpriteDrawMode drawMode;

        public bool flipX;
        public bool flipY;
        public Material material;

        public Vector2 size;
        public Sprite sprite;
        public SpriteTileMode tileMode;

        public VirtualSpriteRenderer()
        {
        }

        public VirtualSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            Set(spriteRenderer);
        }

        public void Set(SpriteRenderer spriteRenderer)
        {
            sprite = spriteRenderer.sprite;

            flipX = spriteRenderer.flipX;
            flipY = spriteRenderer.flipY;

            tileMode = spriteRenderer.tileMode;
            drawMode = spriteRenderer.drawMode;

            size = spriteRenderer.size;

            //material = spriteRenderer.sharedMaterial;

            color = spriteRenderer.color;
        }
    }
}