using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.LightTilemapCollider;
using FunkyCode.SuperTilemapEditorSupport;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightTilemapRoom2D : MonoBehaviour
    {
        public enum MaskType
        {
            Sprite
        } // Separate For Each Map Type!

        public enum ShaderType
        {
            ColorMask,
            MultiplyTexture
        }

        public static List<LightTilemapRoom2D> List = new();
        public int lightLayer;

        public MapType mapType = MapType.UnityRectangle;
        public MaskType maskType = MaskType.Sprite;
        public ShaderType shaderType = ShaderType.ColorMask;
        public Color color = Color.black;
        public Rectangle rectangle = new();

        public LightingTilemapRoomTransform lightingTransform = new();

        public TilemapRoom2D superTilemapEditor = new();

        public void Update()
        {
            lightingTransform.Update(this);

            if (lightingTransform.UpdateNeeded)
            {
                GetCurrentTilemap().ResetWorld();

                Light2D.ForceUpdateAll();
            }
        }

        public void OnEnable()
        {
            List.Add(this);

            LightingManager2D.Get();

            rectangle.SetGameObject(gameObject);
            superTilemapEditor.SetGameObject(gameObject);

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

        public Base GetCurrentTilemap()
        {
            switch (mapType)
            {
                case MapType.SuperTilemapEditor:
                    return superTilemapEditor;
                case MapType.UnityRectangle:
                    return rectangle;
            }

            return null;
        }

        public void Initialize()
        {
            TilemapEvents.Initialize();

            GetCurrentTilemap().Initialize();
        }

        public TilemapProperties GetTilemapProperties()
        {
            return GetCurrentTilemap().Properties;
        }

        public List<LightTile> GetTileList()
        {
            return GetCurrentTilemap().MapTiles;
        }

        public float GetRadius()
        {
            return GetCurrentTilemap().GetRadius();
        }

        private void DrawGizmos()
        {
            if (!isActiveAndEnabled) return;

            // Gizmos.color = new Color(1f, 0.5f, 0.25f);

            Gizmos.color = new Color(0, 1f, 1f);

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    GizmosHelper.DrawRect(transform.position, GetCurrentTilemap().GetRect());

                    break;
            }
        }
    }
}