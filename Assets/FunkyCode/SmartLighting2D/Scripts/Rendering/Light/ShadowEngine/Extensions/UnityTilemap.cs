using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public class UnityTilemap
    {
        public static void Draw(Light2D light, LightTilemapCollider2D id)
        {
            Vector2 lightPosition = -light.transform.position;
            var tilemapCollider = id.GetCurrentTilemap();

            var count = tilemapCollider.chunkManager.GetTiles(light.transform2D.WorldRect);

            Vector2 localPosition;

            for (var i = 0; i < count; i++)
            {
                var tile = tilemapCollider.chunkManager.display[i];

                if (tile.occluded) continue;

                switch (id.shadowTileType)
                {
                    case ShadowTileType.AllTiles:
                        break;

                    case ShadowTileType.ColliderOnly:
                        if (tile.colliderType == UnityEngine.Tilemaps.Tile.ColliderType.None) continue;
                        break;
                }

                var polygons = tile.GetWorldPolygons(tilemapCollider);
                var tilePosition = tile.GetWorldPosition(tilemapCollider);

                localPosition.x = lightPosition.x + tilePosition.x;
                localPosition.y = lightPosition.y + tilePosition.y;

                if (tile.NotInRange(localPosition, light.size)) continue;

                ShadowEngine.Draw(polygons, 0, 0, id.shadowTranslucency);
            }
        }
    }
}