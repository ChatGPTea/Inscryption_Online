using UnityEngine;

namespace FunkyCode
{
    public class SpriteTiledMesh
    {
        private readonly MeshBrush brush;
        private Mesh cacheMesh;
        private MeshObject cacheMeshObject;
        private Vector2 cacheSize;

        private Sprite cacheSprite;
        private readonly SpriteMesh spriteMesh;

        public SpriteTiledMesh()
        {
            brush = new MeshBrush();
            spriteMesh = new SpriteMesh();
        }

        public MeshObject GetMesh(SpriteRenderer spriteRenderer)
        {
            if (!cacheSize.Equals(spriteRenderer.size) || !cacheSprite.Equals(spriteRenderer.sprite))
            {
                cacheMesh = Generate(spriteRenderer);
                cacheMeshObject = MeshObject.Get(cacheMesh);

                cacheSize = spriteRenderer.size;
                cacheSprite = spriteRenderer.sprite;
            }

            return cacheMeshObject;
        }

        private Mesh Generate(SpriteRenderer spriteRenderer)
        {
            brush.Clear();

            var spriteRect = spriteRenderer.sprite.textureRect;

            var spriteRatioX = spriteRect.width / spriteRenderer.sprite.texture.width;
            var spriteRatioY = spriteRect.height / spriteRenderer.sprite.texture.height;

            var stretchX = spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit;
            var stretchY = spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit;

            float scaleX, scaleY;

            var sizeX = Mathf.Abs(spriteRenderer.size.x) / spriteRatioX;
            var sizeY = Mathf.Abs(spriteRenderer.size.y) / spriteRatioY;

            float borderX0 = 0;
            var borderX1 = spriteRenderer.sprite.border.z / spriteRect.width;

            float borderY0;
            var borderY1 = spriteRenderer.sprite.border.w / spriteRect.height;

            var fullX = 1f - borderX1;
            float fullY;

            var sizeLeftX = sizeX / stretchX;
            float offset_x = 0;

            float sizeLeftY;
            float offset_y;

            while (sizeLeftX > 0)
            {
                scaleX = sizeLeftX > fullX ? scaleX = fullX : scaleX = sizeLeftX;

                if (sizeLeftX > fullX)
                {
                    sizeLeftX -= fullX;

                    var sizeOffsetX = offset_x - sizeLeftX / 2 * stretchX * spriteRatioX;

                    sizeLeftY = sizeY / stretchY;
                    offset_y = 0;

                    borderY0 = 0;
                    fullY = 1f - borderY1;

                    while (sizeLeftY > 0)
                    {
                        scaleY = sizeLeftY > fullY ? scaleY = fullY : scaleY = sizeLeftY;

                        if (sizeLeftY > fullY)
                        {
                            sizeLeftY -= fullY;

                            var sizeOffsetY = offset_y - sizeLeftY / 2 * stretchY * spriteRatioY;

                            brush.AddMesh(
                                spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),
                                    new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)),
                                new Vector3(sizeOffsetX, sizeOffsetY, 0));
                        }
                        else
                        {
                            brush.AddMesh(
                                spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),
                                    new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)),
                                new Vector3(sizeOffsetX, offset_y, 0));

                            sizeLeftY -= fullY;
                        }

                        offset_y += fullY / 2 * stretchY * spriteRatioY;

                        borderY0 = spriteRenderer.sprite.border.y / spriteRect.height;
                        fullY = 1f - borderY1 - borderY0;
                    }
                }
                else
                {
                    sizeLeftY = sizeY / stretchY;
                    offset_y = 0;

                    borderY0 = 0;
                    fullY = 1f - borderY1;

                    while (sizeLeftY > 0)
                    {
                        scaleY = sizeLeftY > fullY ? scaleY = fullY : scaleY = sizeLeftY;

                        if (sizeLeftY > fullY)
                        {
                            sizeLeftY -= fullY;

                            var sizeOffsetY = offset_y - sizeLeftY / 2 * stretchY * spriteRatioY;

                            brush.AddMesh(
                                spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),
                                    new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)),
                                new Vector3(offset_x, sizeOffsetY, 0));
                        }
                        else
                        {
                            brush.AddMesh(
                                spriteMesh.Get(spriteRenderer, new Vector2(scaleX, scaleY),
                                    new Vector2(borderX0, borderY0), new Vector2(scaleX, scaleY)),
                                new Vector3(offset_x, offset_y, 0));

                            sizeLeftY -= fullY;
                        }

                        offset_y += fullY / 2 * stretchY * spriteRatioY;


                        borderY0 = spriteRenderer.sprite.border.y / spriteRect.height;
                        fullY = 1f - borderY1 - borderY0;
                    }

                    sizeLeftX -= fullX;
                }

                offset_x += fullX / 2 * stretchX * spriteRatioX;

                borderX0 = spriteRenderer.sprite.border.x / spriteRect.width;
                fullX = 1f - borderX1 - borderX0;
            }

            return brush.Export();
        }
    }
}