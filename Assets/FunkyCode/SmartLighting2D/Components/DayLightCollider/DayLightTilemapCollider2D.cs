using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.LightTilemapCollider;
using FunkyCode.SuperTilemapEditorSupport;
using FunkyCode.Utilities;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    public class DayLightingTile
    {
        public float height = 1;
        public List<Polygon2> polygons;

        public Rect rect;

        public Rect GetDayRect()
        {
            if (rect.width <= 0) rect = Polygon2Helper.GetDayRect(polygons, height);

            return rect;
        }

        public bool InCamera(Camera camera)
        {
            var cameraRect = CameraTransform.GetWorldRect(camera);
            var tileRect = GetDayRect();

            if (cameraRect.Overlaps(tileRect)) return true;

            return false;
        }
    }

    [ExecuteInEditMode]
    public class DayLightTilemapCollider2D : MonoBehaviour
    {
        public enum MaskLit
        {
            Lit,
            Unlit
        }

        public MapType tilemapType = MapType.UnityRectangle;

        public int shadowLayer;

        public float shadowTranslucency;

        public float shadowSoftness;

        public ShadowTileType shadowTileType = ShadowTileType.AllTiles;

        public float height = 1;

        public int maskLayer;

        public MaskLit maskLit = MaskLit.Lit;

        public Rectangle rectangle = new();
        public Isometric isometric = new();
        public Hexagon hexagon = new();

        public TilemapCollider2D superTilemapEditor = new();

        public List<DayLightingTile> dayTiles = new();

        public DayLightTilemapColliderTransform transform2D = new();
        public static List<DayLightTilemapCollider2D> List { get; } = new();

        private void Update()
        {
            transform2D.Update(this);

            if (transform2D.moved)
            {
                transform2D.moved = false;

                foreach (var dayTile in dayTiles) dayTile.height = height;
            }
        }

        public void OnEnable()
        {
            List.Add(this);

            rectangle.SetGameObject(gameObject);
            isometric.SetGameObject(gameObject);
            hexagon.SetGameObject(gameObject);

            superTilemapEditor.eventsInit = false;
            superTilemapEditor.SetGameObject(gameObject);

            LightingManager2D.Get();

            Initialize();
        }

        public void OnDisable()
        {
            List.Remove(this);
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

        public bool ShadowsDisabled()
        {
            return GetCurrentTilemap().ShadowsDisabled();
        }

        public bool MasksDisabled()
        {
            return GetCurrentTilemap().MasksDisabled();
        }

        public Base GetCurrentTilemap()
        {
            switch (tilemapType)
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
            TilemapEvents.Initialize();

            GetCurrentTilemap().Initialize();

            dayTiles.Clear();

            switch (tilemapType)
            {
                case MapType.SuperTilemapEditor:

                    switch (superTilemapEditor.shadowTypeSTE)
                    {
                        case TilemapCollider.ShadowType.Grid:
                        case TilemapCollider.ShadowType.TileCollider:
                            foreach (var tile in GetTileList())
                            {
                                var dayTile = new DayLightingTile();
                                dayTile.height = height;

                                dayTile.polygons = tile.GetWorldPolygons(GetCurrentTilemap());

                                dayTiles.Add(dayTile);
                            }

                            break;

#if (SUPER_TILEMAP_EDITOR)
						case SuperTilemapEditorSupport.TilemapCollider.ShadowType.Collider:
						
								foreach(Polygon2 polygon in superTilemapEditor.GetWorldColliders()) {
									DayLightingTile dayTile = new DayLightingTile();

									dayTile.height = height;

									dayTile.polygons = new List<Polygon2>();
									
									Polygon2 poly = polygon.Copy();
									poly.ToOffsetSelf(transform.position);

									dayTile.polygons.Add(poly);
								
									dayTiles.Add(dayTile);

								}

							
						break;

#endif
                    }


                    break;

                case MapType.UnityRectangle:

                    switch (rectangle.shadowType)
                    {
                        case ShadowType.Grid:
                        case ShadowType.SpritePhysicsShape:

                            foreach (var tile in GetTileList())
                            {
                                var dayTile = new DayLightingTile();
                                dayTile.height = height;

                                dayTile.polygons = tile.GetWorldPolygons(GetCurrentTilemap());

                                dayTiles.Add(dayTile);
                            }

                            break;

                        case ShadowType.CompositeCollider:

                            foreach (var polygon in rectangle.compositeColliders)
                            {
                                var dayTile = new DayLightingTile();
                                dayTile.height = height;

                                dayTile.polygons = new List<Polygon2>();

                                var poly = polygon.Copy();
                                poly.ToOffsetSelf(transform.position);

                                dayTile.polygons.Add(poly);

                                dayTiles.Add(dayTile);
                            }

                            break;
                    }

                    break;
            }
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

            Gizmos.color = new Color(1f, 0.5f, 0.25f);

            var tilemap = GetCurrentTilemap();

            foreach (var dayTile in dayTiles) GizmosHelper.DrawPolygons(dayTile.polygons, transform.position);

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    Gizmos.color = new Color(0, 1f, 1f, 0.25f);

                    foreach (var dayTile in dayTiles) GizmosHelper.DrawRect(Vector2.zero, dayTile.GetDayRect());

                    Gizmos.color = new Color(0, 1f, 1f, 0.5f);

                    GizmosHelper.DrawRect(transform.position, tilemap.GetRect());

                    break;
            }
        }
    }
}