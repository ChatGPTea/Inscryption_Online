using System.Collections.Generic;
using FunkyCode.LightingSettings;
using UnityEngine;

namespace FunkyCode.Rendering.Depth
{
    public class Pass
    {
        public Camera camera;
        public int colliderCount;

        public List<DayLightCollider2D> colliderList;
        public LightmapLayer layer;
        public int layerId;
        public Vector2 offset;
        public int tilemapColliderCount;

        // public bool drawShadows = false;
        // public bool drawMask = false;

        public List<DayLightTilemapCollider2D> tilemapColliderList;

        public bool Setup(LightmapLayer slayer, Camera camera)
        {
            if (slayer.id < 0) return false;

            layerId = slayer.id;
            layer = slayer;

            this.camera = camera;
            offset = -camera.transform.position;

            colliderList = DayLightCollider2D.List;
            colliderCount = colliderList.Count;

            tilemapColliderList = DayLightTilemapCollider2D.List;
            tilemapColliderCount = tilemapColliderList.Count;

            // drawShadows = slayer.type != LayerType.MaskOnly;
            // drawMask = slayer.type != LayerType.ShadowsOnly;

            return true;
        }
    }
}