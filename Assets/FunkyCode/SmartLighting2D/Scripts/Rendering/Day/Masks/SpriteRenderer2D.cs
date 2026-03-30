using FunkyCode.LightTilemapCollider;
using FunkyCode.Utilities;
using UnityEngine;
using Sprite = FunkyCode.Rendering.Universal.Sprite;

namespace FunkyCode.Rendering.Day
{
    public static class SpriteRenderer2D
    {
        public static Texture2D currentTexture;

        private static readonly VirtualSpriteRenderer virtualSpriteRenderer = new();

        public static void Draw(DayLightCollider2D id, Vector2 offset)
        {
            if (!id.InAnyCamera()) return;

            var material = Lighting2D.Materials.mask.GetDayMask();

            GLExtended.color = DayMaskColor.Get(id);

            var shape = id.mainShape;

            var spriteRenderer = shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null || spriteRenderer.sprite == null) return;

            var texture = spriteRenderer.sprite.texture;

            if (texture == null) return;

            if (currentTexture != texture)
            {
                if (currentTexture != null) GL.End();

                currentTexture = texture;
                material.mainTexture = currentTexture;

                material.SetPass(0);
                GL.Begin(GL.QUADS);
            }

            var position = shape.transform2D.position;
            position.x += offset.x;
            position.y += offset.y;

            Sprite.Pass.Draw(id.spriteMeshObject, spriteRenderer, position, shape.transform2D.scale,
                shape.transform2D.rotation);
        }

        public static void DrawTilemap(DayLightTilemapCollider2D id, Vector2 offset)
        {
            //if (id.InAnyCamera() == false) {
            //	return;
            //}

            if (id.rectangle.maskType != MaskType.Sprite) return;

            var tilemap = id.GetCurrentTilemap();

            var scale = tilemap.TileWorldScale();
            var rotation = id.transform.eulerAngles.z;

            var material = Lighting2D.Materials.mask.GetMask(); // why not day mask?

            foreach (var tile in id.rectangle.MapTiles)
            {
                if (tile.GetSprite() == null) return;

                tile.UpdateTransform(tilemap);

                var tilePosition = tile.GetWorldPosition(tilemap);

                tilePosition += offset;

                // if (tile.NotInRange(tilePosition, light.size)) {
                //   continue;
                //}

                virtualSpriteRenderer.sprite = tile.GetSprite();

                GLExtended.color = Color.white;

                material.mainTexture = virtualSpriteRenderer.sprite.texture;

                if (id.maskLit == DayLightTilemapCollider2D.MaskLit.Lit)
                    GLExtended.color = Color.white;
                else
                    GLExtended.color = Color.black;

                material.SetPass(0);

                Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, virtualSpriteRenderer, tilePosition, scale,
                    rotation);

                material.mainTexture = null;
            }
        }

        public static void DrawBumped(DayLightCollider2D id, Vector2 offset)
        {
            if (!id.InAnyCamera()) return;

            var bumpTexture = id.normalMapMode.GetBumpTexture();

            if (bumpTexture == null) return;

            var dayLightRotation = -(Lighting2D.DayLightingSettings.direction - 180) * Mathf.Deg2Rad;
            var dayLightHeight = Lighting2D.DayLightingSettings.bumpMap.height;
            var dayLightStrength = Lighting2D.DayLightingSettings.bumpMap.strength;

            var material = Lighting2D.Materials.bumpMask.GetBumpedDaySprite();
            material.SetFloat("_LightRZ", -dayLightHeight);
            material.SetTexture("_Bump", bumpTexture);

            var shape = id.mainShape;

            var spriteRenderer = shape.spriteShape.GetSpriteRenderer();

            if (spriteRenderer == null || spriteRenderer.sprite == null) return;

            var rotation = dayLightRotation - shape.transform2D.rotation * Mathf.Deg2Rad;
            material.SetFloat("_LightRX", Mathf.Cos(rotation) * dayLightStrength);
            material.SetFloat("_LightRY", Mathf.Sin(rotation) * dayLightStrength);

            var objectOffset = shape.transform2D.position + offset;

            material.mainTexture = spriteRenderer.sprite.texture;

            material.SetPass(0);

            Sprite.FullRect.Draw(id.spriteMeshObject, spriteRenderer, objectOffset, id.transform.lossyScale,
                shape.transform2D.rotation);
        }
    }
}