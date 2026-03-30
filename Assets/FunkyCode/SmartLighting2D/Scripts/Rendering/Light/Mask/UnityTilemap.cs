using FunkyCode.LightSettings;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Light
{
    public class UnityTilemap
    {
        public static VirtualSpriteRenderer virtualSpriteRenderer = new();

        public static void Sprite(Light2D light, LightTilemapCollider2D id, Material material,
            LayerSetting layerSetting)
        {
            Vector2 lightPosition = -light.transform.position;

            var tilemap = id.GetCurrentTilemap();
            var scale = tilemap.TileWorldScale();
            var localScale = Vector2.zero;
            var rotation = id.transform.eulerAngles.z;

            var count = tilemap.chunkManager.GetTiles(light.transform2D.WorldRect);

            Texture2D currentTexture = null;

            for (var i = 0; i < count; i++)
            {
                var tile = tilemap.chunkManager.display[i];

                if (tile.GetSprite() == null) return;

                var tilePosition = tile.GetWorldPosition(tilemap);

                tilePosition.x += lightPosition.x;
                tilePosition.y += lightPosition.y;

                if (tile.NotInRange(tilePosition, light.size)) continue;

                virtualSpriteRenderer.sprite = tile.GetSprite();

                var texture = virtualSpriteRenderer.sprite.texture;

                if (texture == null) continue;

                if (currentTexture != texture)
                {
                    if (currentTexture != null) GL.End();

                    currentTexture = texture;
                    material.mainTexture = currentTexture;

                    material.SetPass(0);
                    GL.Begin(GL.QUADS);
                }

                GLExtended.color = LayerSettingColor.Get(tilePosition, layerSetting, MaskLit.Lit, 1, 1);

                localScale.x = scale.x * tile.scale.x;
                localScale.y = scale.y * tile.scale.y;

                Universal.Sprite.Pass.Draw(tile.spriteMeshObject, virtualSpriteRenderer, tilePosition, localScale,
                    rotation + tile.rotation);
            }

            if (currentTexture != null) GL.End();

            material.mainTexture = null;
        }

        public static void BumpedSprite(Light2D light, LightTilemapCollider2D id, Material material,
            LayerSetting layerSetting)
        {
            var bumpTexture = id.bumpMapMode.GetBumpTexture();

            if (bumpTexture == null) return;

            material.SetTexture("_Bump", bumpTexture);

            Vector2 lightPosition = -light.transform.position;

            var tilemap = id.GetCurrentTilemap();
            var scale = tilemap.TileWorldScale();

            Texture2D currentTexture = null;

            GL.Begin(GL.QUADS);

            // Optimize Bumped Sprites?

            var count = tilemap.chunkManager.GetTiles(light.transform2D.WorldRect);

            for (var i = 0; i < count; i++)
            {
                var tile = tilemap.chunkManager.display[i];

                if (tile.GetSprite() == null) return;

                var tilePosition = tilemap.TileWorldPosition(tile);

                tilePosition += lightPosition;

                if (tile.NotInRange(tilePosition, light.size)) continue;

                virtualSpriteRenderer.sprite = tile.GetSprite();

                if (virtualSpriteRenderer.sprite.texture == null) continue;

                if (currentTexture != virtualSpriteRenderer.sprite.texture)
                {
                    currentTexture = virtualSpriteRenderer.sprite.texture;
                    material.mainTexture = currentTexture;

                    material.SetPass(0);
                }

                GLExtended.color = LayerSettingColor.Get(tilePosition, layerSetting, MaskLit.Lit, 1, 1); // 1

                Universal.Sprite.FullRect.Simple.Draw(tile.spriteMeshObject, virtualSpriteRenderer, tilePosition, scale,
                    tile.worldRotation);
            }

            GL.End();

            material.mainTexture = null;
        }

        public static void MaskShape(Light2D light, LightTilemapCollider2D id, LayerSetting layerSetting)
        {
            Vector2 lightPosition = -light.transform.position;

            var tilemap = id.GetCurrentTilemap();

            var isGrid = !tilemap.IsPhysicsShape();

            var scale = tilemap.TileWorldScale();

            var rotation = id.transform.eulerAngles.z;

            MeshObject tileMesh = null;

            if (isGrid) tileMesh = LightTile.GetStaticMesh(tilemap);

            var count = tilemap.chunkManager.GetTiles(light.transform2D.WorldRect);

            for (var i = 0; i < count; i++)
            {
                var tile = tilemap.chunkManager.display[i];

                var tilePosition = tilemap.TileWorldPosition(tile);

                tilePosition += lightPosition;

                if (tile.NotInRange(tilePosition, light.size)) continue;

                if (!isGrid)
                {
                    tileMesh = null;
                    tileMesh = tile.GetDynamicMesh();
                }

                if (tileMesh == null) continue;

                GLExtended.color = LayerSettingColor.Get(tilePosition, layerSetting, MaskLit.Lit, 1, 1); // 1?

                GLExtended.DrawMeshPass(tileMesh, tilePosition, scale, rotation + tile.rotation);
            }

            GL.Color(Color.white);
        }
    }
}