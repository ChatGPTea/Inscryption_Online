using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode
{
    public class TilemapProperties
    {
        public BoundsInt area;
        public Vector2 cellAnchor = new(0.5f, 0.5f);
        public Vector2 cellGap = new(1, 1);
        public Vector2 cellSize = new(1, 1);
        public Vector2 colliderOffset = new(0, 0);
        public Grid grid;

        public Tilemap tilemap;

        public Transform transform;
    }
}