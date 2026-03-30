using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using FunkyCode.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode.LightTilemapCollider
{
    [Serializable]
    public class Rectangle : Base
    {
        public bool shadowOptimization;
        public List<Polygon2> compositeColliders = new();

        private LightTilemapCollider2D lightTilemapCollider2D;

        private Tilemap tilemap2D;

        public override MapType TilemapType()
        {
            return MapType.UnityRectangle;
        }

        public static ITilemap GetITilemap(Tilemap tilemap)
        {
            var iTilemap = (ITilemap)FormatterServices.GetUninitializedObject(typeof(ITilemap));
            typeof(ITilemap).GetField("m_Tilemap", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(iTilemap, tilemap);
            return iTilemap;
        }

        // That is not complete
        public override bool IsPhysicsShape()
        {
            if (maskType == MaskType.SpritePhysicsShape) return true;

            if (shadowType == ShadowType.SpritePhysicsShape) return true;

            return false;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (!UpdateProperties()) return;

            // Tilemap tilemap2D = properties.tilemap; ????/

            lightTilemapCollider2D = gameObject.GetComponent<LightTilemapCollider2D>();

            var tilemapCollider = gameObject.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null) properties.colliderOffset = tilemapCollider.offset;

            properties.cellAnchor += properties.colliderOffset;

            InitializeGrid();
            InitializeCompositeCollider();

            chunkManager.Update(MapTiles, this);
        }

        private void InitializeCompositeCollider()
        {
            compositeColliders.Clear();

            var compositeCollider2D = gameObject.GetComponent<CompositeCollider2D>();

            if (compositeCollider2D != null)
                compositeColliders = Polygon2Collider2D.CreateFromCompositeCollider(compositeCollider2D);
        }

        public override Vector2 TileWorldPosition(LightTile tile)
        {
            Vector2 position = tilemap2D.CellToWorld(tile.gridPosition);

            var rotation = properties.cellAnchor.Atan2() + tilemap2D.transform.eulerAngles.z * Mathf.Deg2Rad;

            var sizeX = properties.cellAnchor.x * tilemap2D.transform.localScale.x;
            var sizeY = properties.cellAnchor.y * tilemap2D.transform.localScale.y;

            var distance = Mathf.Sqrt(sizeX * sizeX + sizeY * sizeY);

            // +++ Include Cell Size

            position.x += properties.grid.cellSize.x - 1;
            position.y += properties.grid.cellSize.y - 1;

            position.x += sizeX;
            position.y += sizeY;

            return position;
        }

        public override float TileWorldRotation(LightTile tile)
        {
            var worldRotation = tilemap2D.transform.eulerAngles.z;

            return worldRotation;
        }

        public override Vector2 TileWorldScale()
        {
            var transform = properties.transform;

            var scale = Vector2.one;

            scale.x *= transform.lossyScale.x;
            scale.y *= transform.lossyScale.y;

            var isGrid = false;
            if (isGrid)
            {
                scale.x *= properties.cellSize.x;
                scale.y *= properties.cellSize.y;
            }

            return scale;
        }

        public void RefreshTile(Vector3Int positionInt)
        {
            var refreshTile = GetTileToRefresh(positionInt);

            var tilemap = GetITilemap(tilemap2D);

            var tilebase = tilemap.GetTile(positionInt);

            var tileData = new TileData();

            if (refreshTile != null)
            {
                if (tilebase != null)
                {
                    tilebase.GetTileData(positionInt, tilemap, ref tileData);

                    var matrix = tilemap.GetTransformMatrix(positionInt);

                    refreshTile.ResetLocal();

                    refreshTile.rotation = matrix.rotation.eulerAngles.z;

                    refreshTile.scale = matrix.lossyScale;

                    refreshTile.SetSprite(tileData.sprite);
                    refreshTile.GetPhysicsShapePolygons();

                    refreshTile.occluded = false;

                    refreshTile.colliderType = tileData.colliderType;
                }
                else
                {
                    lightTilemapCollider2D.rectangle.MapTiles.Remove(refreshTile);
                }
            }
            else
            {
                if (tilebase != null)
                {
                    var lightTile = new LightTile();

                    lightTile.gridPosition = positionInt;

                    tilebase.GetTileData(positionInt, tilemap, ref tileData);

                    var matrix = tilemap.GetTransformMatrix(positionInt);

                    lightTile.rotation = matrix.rotation.eulerAngles.z;

                    lightTile.scale = matrix.lossyScale;

                    lightTile.SetSprite(tileData.sprite);
                    lightTile.GetPhysicsShapePolygons();

                    lightTile.colliderType = tileData.colliderType;

                    lightTilemapCollider2D.rectangle.MapTiles.Add(lightTile);
                }
            }

            chunkManager.Update(MapTiles, lightTilemapCollider2D.GetCurrentTilemap());

            Light2D.ForceUpdateAll();
        }

        public LightTile GetTileToRefresh(Vector3Int gridPosition)
        {
            foreach (var tile in MapTiles)
                if (tile.gridPosition == gridPosition)
                    return tile;

            return null;
        }

        private void InitializeGrid()
        {
            MapTiles.Clear();

            tilemap2D = properties.tilemap;
            var tilemap = GetITilemap(tilemap2D);

            foreach (var position in tilemap2D.cellBounds.allPositionsWithin)
            {
                var tileData = new TileData();

                var tilebase = tilemap2D.GetTile(position);

                if (tilebase != null)
                {
                    tilebase.GetTileData(position, tilemap, ref tileData);

                    var lightTile = new LightTile();

                    lightTile.gridPosition = position;

                    if (shadowOptimization)
                    {
                        var left = GetTile(position + new Vector3Int(1, 0, 0));
                        var up = GetTile(position + new Vector3Int(0, 1, 0));
                        var right = GetTile(position + new Vector3Int(-1, 0, 0));
                        var down = GetTile(position + new Vector3Int(0, -1, 0));

                        lightTile.occluded = left && right && up && down;
                    }

                    var matrix = tilemap2D.GetTransformMatrix(position);

                    lightTile.rotation = matrix.rotation.eulerAngles.z;

                    lightTile.scale = matrix.lossyScale;

                    lightTile.SetSprite(tileData.sprite);
                    lightTile.GetPhysicsShapePolygons();

                    lightTile.colliderType = tileData.colliderType;

                    MapTiles.Add(lightTile);
                }
            }
        }

        public bool GetTile(Vector3Int position)
        {
            var tilebase = tilemap2D.GetTile(position);

            if (tilebase == null) return false;

            var tileData = new TileData();

            var tilemap = GetITilemap(tilemap2D);
            tilebase.GetTileData(position, tilemap, ref tileData);

            if (tileData.sprite == null) return false;

            return true;
        }
    }
}