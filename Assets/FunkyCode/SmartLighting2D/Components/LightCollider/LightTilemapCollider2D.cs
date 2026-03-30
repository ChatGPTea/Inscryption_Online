using System.Collections.Generic;
using FunkyCode.Chunks;
using FunkyCode.LightingSettings;
using FunkyCode.LightTilemapCollider;
using FunkyCode.SuperTilemapEditorSupport;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    public enum ShadowTileType
    {
        AllTiles,
        ColliderOnly
    }

    [ExecuteInEditMode]
    public class LightTilemapCollider2D : MonoBehaviour
    {
        public static List<LightTilemapCollider2D> List = new();
        public static LightColliderLayer<LightTilemapCollider2D> layerManagerMask = new();
        public static LightColliderLayer<LightTilemapCollider2D> layerManagerCollision = new();
        public MapType mapType = MapType.UnityRectangle;

        public int shadowLayer;
        public int maskLayer;

        public float shadowTranslucency;

        public ShadowTileType shadowTileType = ShadowTileType.AllTiles;

        public BumpMapMode bumpMapMode = new();

        public Rectangle rectangle = new();
        public Isometric isometric = new();
        public Hexagon hexagon = new();

        public TilemapCollider2D superTilemapEditor = new();

        public LightTilemapTransform lightingTransform = new();
        private int listCollisionLayer = -1;

        private int listMaskLayer = -1;

        public void Update()
        {
            UpdateLayerList();

            lightingTransform.Update(this);

            if (lightingTransform.UpdateNeeded)
            {
                GetCurrentTilemap().ResetWorld();

                // Update if light is in range
                foreach (var light in Light2D.List)
                    //if (IsInRange(light)) {
                    light.ForceUpdate();
                //}
            }
        }

        public void OnEnable()
        {
            List.Add(this);

            UpdateLayerList();

            LightingManager2D.Get();

            rectangle.SetGameObject(gameObject);
            isometric.SetGameObject(gameObject);
            hexagon.SetGameObject(gameObject);

            superTilemapEditor.eventsInit = false;
            superTilemapEditor.SetGameObject(gameObject);

            Initialize();

            Light2D.ForceUpdateAll();
        }

        public void OnDisable()
        {
            List.Remove(this);

            ClearLayerList();

            Light2D.ForceUpdateAll();
        }

        private void OnDrawGizmos()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Always) return;

            DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Selected) return;

            DrawGizmos();
        }

        public static List<LightTilemapCollider2D> GetMaskList(int layer)
        {
            return layerManagerMask.layerList[layer];
        }

        public static List<LightTilemapCollider2D> GetShadowList(int layer)
        {
            return layerManagerCollision.layerList[layer];
        }

        // Layer List
        private void ClearLayerList()
        {
            layerManagerMask.Remove(listMaskLayer, this);
            layerManagerCollision.Remove(listCollisionLayer, this);

            listMaskLayer = -1;
            listCollisionLayer = -1;
        }

        private void UpdateLayerList()
        {
            listMaskLayer = layerManagerMask.Update(listMaskLayer, maskLayer, this);
            listCollisionLayer = layerManagerCollision.Update(listCollisionLayer, shadowLayer, this);
        }

        public bool ShadowsDisabled()
        {
            return GetCurrentTilemap().ShadowsDisabled();
        }

        public bool MasksDisabled()
        {
            return GetCurrentTilemap().MasksDisabled();
        }

        public bool InLight(Light2D light)
        {
            var tilemapRect = GetCurrentTilemap().GetRect();
            var lightRect = light.transform2D.WorldRect;

            return tilemapRect.Overlaps(lightRect);
        }

        public void RefreshTile(Vector3Int position)
        {
            switch (mapType)
            {
                case MapType.UnityRectangle:
                    rectangle.RefreshTile(position);
                    break;
            }
        }

        /*
        public bool IsInRange(Light2D light) {
            float radius = GetCurrentTilemap().GetRadius() + light.size;
            float distance = Vector2.Distance(light.transform.position, transform.position);

            return(distance < radius);
        }*/

        //public bool IsNotInRange(Light2D light) {
        //float radius = GetCurrentTilemap().GetRadius() + light.size;
        //float distance = Vector2.Distance(light.transform.position, transform.position);

        //return(distance > radius);

        //	return(false);
        //}

        public Base GetCurrentTilemap()
        {
            switch (mapType)
            {
                case MapType.SuperTilemapEditor:
                    return superTilemapEditor;

                case MapType.UnityRectangle:
                    return rectangle;

                case MapType.UnityIsometric:
                    return isometric;

                case MapType.UnityHexagon:
                    return hexagon;
            }

            return null;
        }

        public void Initialize()
        {
            rectangle.SetGameObject(gameObject);
            isometric.SetGameObject(gameObject);
            hexagon.SetGameObject(gameObject);

            TilemapEvents.Initialize();

            GetCurrentTilemap().Initialize();
        }

        public List<LightTile> GetTileList()
        {
            return GetCurrentTilemap().MapTiles;
        }

        public TilemapProperties GetTilemapProperties()
        {
            return GetCurrentTilemap().Properties;
        }

        private void DrawGizmos()
        {
            if (!isActiveAndEnabled) return;

            var tilemap = GetCurrentTilemap();

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosShadowCasters)
            {
                case EditorShadowCasters.Enabled:

                    Gizmos.color = new Color(1f, 0.5f, 0.25f);

                    foreach (var tile in GetTileList())
                        GizmosHelper.DrawPolygons(tile.GetWorldPolygons(tilemap), transform.position);

                    // GizmosHelper.DrawPolygons(superTilemapEditor.GetWorldColliders(), transform.position);

                    break;
            }

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosChunks)
            {
                case EditorChunks.Enabled:

                    Gizmos.color = new Color(1, 0.5f, 0.75f);

                    var rect = GetCurrentTilemap().GetRect();

                    var pos0 = TilemapManager.TransformBounds(new Vector2(rect.x, rect.y));
                    var pos1 = TilemapManager.TransformBounds(new Vector2(rect.x + rect.width, rect.y + rect.height));

                    // Lighting2D.ProjectSettings.chunks.chunkSize
                    var chunkSize = TilemapManager.ChunkSize;

                    for (var i = pos0.x; i <= pos1.x + 1; i++)
                    {
                        var lineA = new Vector2(i * chunkSize, pos0.y * chunkSize);
                        var lineB = new Vector2(i * chunkSize, (pos1.y + 1) * chunkSize);
                        Gizmos.DrawLine(lineA, lineB);
                    }

                    for (var i = pos0.y; i <= pos1.y + 1; i++)
                    {
                        var lineA = new Vector2(pos0.x * chunkSize, i * chunkSize);
                        var lineB = new Vector2((pos1.x + 1) * chunkSize, i * chunkSize);
                        Gizmos.DrawLine(lineA, lineB);
                    }

                    break;
            }

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    Gizmos.color = new Color(0, 1f, 1f);

                    var rect = GetCurrentTilemap().GetRect();

                    GizmosHelper.DrawRect(transform.position, rect);

                    break;
            }
        }
    }
}