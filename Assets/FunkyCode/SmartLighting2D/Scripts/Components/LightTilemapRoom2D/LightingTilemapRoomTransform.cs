using FunkyCode.LightTilemapCollider;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode
{
    public class LightingTilemapRoomTransform
    {
        private Grid grid;
        public Vector2 position = Vector2.one;
        public float rotation;

        private Vector2 scale = Vector2.one;

        private Tilemap tilemap;
        public Vector3 tilemapAnchor = Vector3.zero;
        public Vector3 tilemapCellSize = Vector3.zero;
        public Vector3 tilemapGapSize = Vector3.zero;

        public bool UpdateNeeded { get; set; } = true;

        public void Update(LightTilemapRoom2D tilemapRoom2D)
        {
            var transform = tilemapRoom2D.transform;

            var position2D = LightingPosition.GetPosition2D(transform.position);
            Vector2 scale2D = transform.lossyScale;
            var rotation2D = transform.rotation.eulerAngles.z;

            UpdateNeeded = false;

            if (scale != scale2D)
            {
                scale = scale2D;

                UpdateNeeded = true;
            }

            if (position != position2D)
            {
                position = position2D;

                UpdateNeeded = true;
            }

            if (rotation != rotation2D)
            {
                rotation = rotation2D;

                UpdateNeeded = true;
            }

            if (tilemapRoom2D.mapType != MapType.SuperTilemapEditor)
            {
                var tilemap = GetTilemap(tilemapRoom2D.gameObject);

                if (tilemap)
                    if (tilemapAnchor != tilemap.tileAnchor)
                    {
                        tilemapAnchor = tilemap.tileAnchor;
                        UpdateNeeded = true;
                    }

                var grid = GetGrid(tilemapRoom2D.gameObject);

                if (grid)
                {
                    if (tilemapCellSize != grid.cellSize)
                    {
                        tilemapCellSize = grid.cellSize;

                        UpdateNeeded = true;
                    }

                    if (tilemapGapSize != grid.cellGap)
                    {
                        tilemapGapSize = grid.cellGap;

                        UpdateNeeded = true;
                    }
                }
            }
        }

        public Tilemap GetTilemap(GameObject gameObject)
        {
            if (tilemap)
                return tilemap;

            tilemap = gameObject.GetComponent<Tilemap>();
            return tilemap;
        }

        public Grid GetGrid(GameObject gameObject)
        {
            if (grid)
                return grid;

            var tilemap = GetTilemap(gameObject);
            if (tilemap) grid = tilemap.layoutGrid;

            return grid;
        }
    }
}