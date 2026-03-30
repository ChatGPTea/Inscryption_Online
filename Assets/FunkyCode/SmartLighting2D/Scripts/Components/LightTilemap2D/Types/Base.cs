using System.Collections.Generic;
using FunkyCode.Chunks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode.LightTilemapCollider
{
    public enum MapType
    {
        UnityRectangle,
        UnityIsometric,
        UnityHexagon,
        SuperTilemapEditor
    }

    public enum ShadowType
    {
        None,
        Grid,
        SpritePhysicsShape,
        CompositeCollider
    }

    public enum MaskType
    {
        None,
        Grid,
        Sprite,
        BumpedSprite,
        SpritePhysicsShape
    }

    public class Base
    {
        public TilemapManager chunkManager = new();

        public GameObject gameObject;

        public MaskType maskType = MaskType.Sprite;
        protected TilemapProperties properties = new();

        private float radius = -1;
        private Rect rect;
        public ShadowType shadowType = ShadowType.Grid;
        public Transform transform;
        public List<LightTile> MapTiles { get; } = new();

        public TilemapProperties Properties => properties;

        // Mask and Shadow Properties
        public bool ShadowsDisabled()
        {
            return shadowType == ShadowType.None;
        }

        public virtual bool MasksDisabled()
        {
            return maskType == MaskType.None;
        }

        // Tile World Properties
        public virtual Vector2 TileWorldPosition(LightTile tile)
        {
            return Vector2.zero;
        }

        public virtual float TileWorldRotation(LightTile tile)
        {
            return 0;
        }

        public virtual Vector2 TileWorldScale()
        {
            return Vector2.one;
        }

        public virtual MapType TilemapType()
        {
            return MapType.UnityRectangle;
        }

        public virtual bool IsPhysicsShape()
        {
            return false;
        }

        public virtual void Initialize()
        {
            radius = -1;
        }

        public void SetGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
            transform = gameObject.transform;

            properties.transform = gameObject.transform;
        }

        public bool UpdateProperties()
        {
            properties.tilemap = gameObject.GetComponent<Tilemap>();

            if (properties.tilemap == null) return false;

            properties.grid = properties.tilemap.layoutGrid;

            if (properties.grid == null)
            {
                Debug.LogError("Lighting 2D Error: Lighting Tilemap Collider is missing Grid", gameObject);
                return false;
            }

            properties.cellSize = properties.grid.cellSize;
            properties.cellGap = properties.grid.cellGap;

            properties.cellAnchor = properties.tilemap.tileAnchor;

            return true;
        }

        public void ResetWorld()
        {
            rect = new Rect();

            foreach (var tile in MapTiles) tile.ResetWorld();
        }

        public Rect GetRect()
        {
            if (rect.width < 0.1f)
            {
                float minX = 100000;
                float minY = 100000;
                float maxX = -100000;
                float maxY = -100000;

                foreach (var tile in MapTiles)
                {
                    var id = tile.GetWorldPosition(this);

                    minX = Mathf.Min(minX, id.x);
                    minY = Mathf.Min(minY, id.y);
                    maxX = Mathf.Max(maxX, id.x);
                    maxY = Mathf.Max(maxY, id.y);
                }

                rect.x = minX;
                rect.y = minY;
                rect.width = maxX - minX;
                rect.height = maxY - minY;
            }

            return rect;
        }

        public float GetRadius()
        {
            if (radius < 0)
                foreach (var tile in MapTiles)
                {
                    var id = tile.GetWorldPosition(this);

                    radius = Mathf.Max(radius, Vector2.Distance(id, gameObject.transform.position));
                }

            return radius;
        }
    }
}