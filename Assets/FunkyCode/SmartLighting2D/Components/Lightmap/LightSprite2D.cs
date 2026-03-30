using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.Utilities;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightSprite2D : MonoBehaviour
    {
        public enum SpriteMode
        {
            Custom,
            SpriteRenderer
        }

        public enum Type
        {
            Light,
            Mask
        }

        public static List<LightSprite2D> List = new();

        private static Sprite defaultSprite;

        public int lightLayer;

        public Type type = Type.Light;
        public SpriteMode spriteMode = SpriteMode.Custom;
        public Sprite sprite;

        public Color color = new(0.5f, 0.5f, 0.5f, 1f);

        public bool flipX;
        public bool flipY;

        public LightSpriteTransform lightSpriteTransform = new();

        public LightSpriteShape lightSpriteShape = new();

        public MeshMode meshMode = new();

        public GlowMode glowMode = new();

        public SpriteMeshObject spriteMeshObject = new();

        public VirtualSpriteRenderer spriteRenderer = new();

        private SpriteRenderer spriteRendererComponent;

        public void OnEnable()
        {
            List.Add(this);

            LightingManager2D.Get();
        }

        public void OnDisable()
        {
            List.Remove(this);
        }

        private void OnDrawGizmos()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos == EditorDrawGizmos.Disabled) return;

            // Gizmos.DrawIcon(transform.position, "light", true);

            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Always) return;

            Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Selected) return;

            Draw();
        }

        public bool InCamera(Camera camera)
        {
            var cameraRect = CameraTransform.GetWorldRect(camera);

            return cameraRect.Overlaps(lightSpriteShape.GetWorldRect());
        }

        public static Sprite GetDefaultSprite()
        {
            if (defaultSprite == null || defaultSprite.texture == null)
                defaultSprite = Resources.Load<Sprite>("Sprites/gfx_light");

            return defaultSprite;
        }

        public Sprite GetSprite()
        {
            if (GetSpriteOrigin() == null) return null;

            return GetSpriteOrigin();
        }

        public Sprite GetSpriteOrigin()
        {
            if (spriteMode == SpriteMode.Custom)
            {
                if (sprite == null) sprite = GetDefaultSprite();

                return sprite;
            }

            if (GetSpriteRenderer() == null) return null;

            sprite = spriteRendererComponent.sprite;

            return sprite;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            if (spriteRendererComponent == null) spriteRendererComponent = GetComponent<SpriteRenderer>();

            return spriteRendererComponent;
        }

        public void UpdateLoop()
        {
            if (spriteMode == SpriteMode.SpriteRenderer)
            {
                var sr = GetSpriteRenderer();

                if (sr != null)
                {
                    spriteRenderer.flipX = sr.flipX;
                    spriteRenderer.flipY = sr.flipY;
                }
            }
            else
            {
                spriteRenderer.flipX = flipX;
                spriteRenderer.flipY = flipY;
            }

            spriteRenderer.sprite = GetSprite();
            spriteRenderer.color = color;

            if (meshMode.enable) DrawMesh();

            lightSpriteShape.Set(spriteRenderer, transform, lightSpriteTransform);

            lightSpriteShape.Update();
        }

        public void DrawMesh()
        {
            if (!meshMode.enable) return;

            var lightingMesh = MeshRendererManager.Pull(this);

            if (lightingMesh != null) lightingMesh.UpdateLightSprite(this, meshMode);
        }

        private void Draw()
        {
            if (!isActiveAndEnabled) return;

            Gizmos.color = new Color(1f, 0.5f, 0.25f);

            GizmosHelper.DrawPolygon(lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

            Gizmos.color = new Color(0, 1f, 1f);

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    GizmosHelper.DrawRect(transform.position, lightSpriteShape.GetWorldRect());

                    break;
            }
        }
    }
}