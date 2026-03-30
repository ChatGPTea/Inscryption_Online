using FunkyCode.LightTilemapCollider;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode
{
    public class LightTilemapTransform
    {
        private Grid grid;
        public Vector2 position = Vector2.one;
        public float rotation;

        private Vector2 scale = Vector2.one;
        public int sortingLayerID;

        public int sortingOrder;

        private Tilemap tilemap;
        public Vector3 tilemapAnchor = Vector3.zero;
        public Vector3 tilemapCellSize = Vector3.zero;
        public Vector3 tilemapGapSize = Vector3.zero;

        public TilemapRenderer tilemapRenderer;

        public bool UpdateNeeded { get; set; } = true;

        public void Update(LightTilemapCollider2D tilemapCollider2D)
        {
            var transform = tilemapCollider2D.transform;

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

            if (tilemapCollider2D.mapType != MapType.SuperTilemapEditor)
            {
                var tilemap = GetTilemap(tilemapCollider2D.gameObject);

                if (tilemap != null)
                    if (tilemapAnchor != tilemap.tileAnchor)
                    {
                        tilemapAnchor = tilemap.tileAnchor;
                        UpdateNeeded = true;
                    }

                var tilemapRenderer = GetTilemapRenderer(tilemapCollider2D.gameObject);

                if (tilemapRenderer != null)
                {
                    var sortID = SortingLayer.GetLayerValueFromID(tilemapRenderer.sortingLayerID);

                    if (sortingLayerID != sortID) sortingLayerID = sortID;

                    if (sortingOrder != tilemapRenderer.sortingOrder) sortingOrder = tilemapRenderer.sortingOrder;
                }

                var grid = GetGrid(tilemapCollider2D.gameObject);

                if (grid != null)
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

        public TilemapRenderer GetTilemapRenderer(GameObject gameObject)
        {
            if (tilemapRenderer == null) tilemapRenderer = gameObject.GetComponent<TilemapRenderer>();

            return tilemapRenderer;
        }

        public Tilemap GetTilemap(GameObject gameObject)
        {
            if (tilemap == null) tilemap = gameObject.GetComponent<Tilemap>();
            return tilemap;
        }

        public Grid GetGrid(GameObject gameObject)
        {
            if (grid == null)
            {
                var tilemap = GetTilemap(gameObject);

                if (tilemap != null) grid = tilemap.layoutGrid;
            }

            return grid;
        }
    }
}