namespace FunkyCode.Rendering.Light.Shadow
{
    public class Tile
    {
        public static void Draw(Light2D light, LightTile tile, LightTilemapCollider2D tilemap)
        {
            var tilemapCollider = tilemap.GetCurrentTilemap();

            var polygons = tile.GetWorldPolygons(tilemapCollider);

            ShadowEngine.Draw(polygons, 0, 0, 0);
        }
    }
}