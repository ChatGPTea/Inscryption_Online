using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using FunkyCode.Rendering.Day.Sorting;
using UnityEngine;

namespace FunkyCode.Rendering.Day
{
    public class Pass
    {
        public Camera camera;
        public int colliderCount;

        public List<DayLightCollider2D> colliderList;
        public bool drawMask;

        public bool drawShadows;
        public LightmapLayer layer;
        public int layerId;
        public Vector2 offset;
        public SortList sortList = new();
        public SortObject sortObject;
        public int tilemapColliderCount;

        public List<DayLightTilemapCollider2D> tilemapColliderList;

        public void SortObjects()
        {
            sortList.Reset();

            var colliderList = DayLightCollider2D.List;

            for (var id = 0; id < colliderList.Count; id++)
            {
                var collider = colliderList[id];
                if (collider.shadowLayer != layerId && collider.maskLayer != layerId)
                    continue;

                switch (layer.sorting)
                {
                    case LayerSorting.ZAxisLower:
                        sortList.Add(collider, -collider.transform.position.z);
                        break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add(collider, collider.transform.position.z);
                        break;
                }

                switch (layer.sorting)
                {
                    case LayerSorting.YAxisLower:
                        sortList.Add(collider, -collider.transform.position.y);
                        break;

                    case LayerSorting.YAxisHigher:
                        sortList.Add(collider, collider.transform.position.y);
                        break;
                }
            }

            var tilemapColliderList = DayLightTilemapCollider2D.List;
            for (var id = 0; id < tilemapColliderList.Count; id++)
            {
                var tilemap = tilemapColliderList[id];

                if (tilemap.shadowLayer != layerId && tilemap.maskLayer != layerId)
                    continue;

                switch (layer.sorting)
                {
                    case LayerSorting.ZAxisLower:
                        sortList.Add(tilemap, -tilemap.transform.position.z);
                        break;

                    case LayerSorting.ZAxisHigher:
                        sortList.Add(tilemap, tilemap.transform.position.z);
                        break;
                }

                switch (layer.sorting)
                {
                    case LayerSorting.YAxisLower:
                        sortList.Add(tilemap, -tilemap.transform.position.y);
                        break;

                    case LayerSorting.YAxisHigher:
                        sortList.Add(tilemap, tilemap.transform.position.y);
                        break;
                }
            }

            sortList.Sort();
        }

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

            drawShadows = slayer.type != LayerType.MaskOnly;
            drawMask = slayer.type != LayerType.ShadowsOnly;

            return true;
        }
    }
}