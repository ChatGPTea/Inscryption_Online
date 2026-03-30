using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.Utilities;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class DayLightCollider2D : MonoBehaviour
    {
        public enum Depth
        {
            None,
            SortingOrder,
            ZPosition,
            YPosition,
            Custom
        }

        public enum DepthFalloff
        {
            Disabled,
            Enabled
        }

        public enum MaskLit
        {
            Lit,
            LitAbove
        }

        public enum MaskType
        {
            None,
            Sprite,
            BumpedSprite
        }

        public enum ShadowEffect
        {
            Softness,
            Falloff
        }

        public enum ShadowType
        {
            None,
            SpritePhysicsShape,
            Collider2D,
            SpriteOffset,
            SpriteProjection,
            SpriteProjectionShape,
            SpriteProjectionCollider,
            FillCollider2D,
            FillSpritePhysicsShape
        }

        public static List<DayLightCollider2D> List = new();

        public int shadowLayer;
        public int maskLayer;

        public ShadowType shadowType = ShadowType.SpritePhysicsShape;
        public MaskType maskType = MaskType.None;

        public ShadowEffect shadowEffect = ShadowEffect.Softness;

        [Min(0)] public float shadowDistance = 1;

        [Min(0)] public float shadowThickness = 1;

        [Min(0)] public float shadowSoftness;

        [Range(0, 1)] public float shadowTranslucency;

        public MaskLit maskLit = MaskLit.Lit;

        public Depth depth = Depth.None;

        public DepthFalloff depthFalloff = DepthFalloff.Disabled;

        public int depthCustomValue;

        public DayLightColliderShape mainShape = new();

        public DayNormalMapMode normalMapMode = new();
        public SpriteMeshObject spriteMeshObject = new();

        public bool isStatic => gameObject.isStatic;

        public void OnEnable()
        {
            List.Add(this);

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

        public int GetDepth()
        {
            switch (depth)
            {
                case Depth.Custom:

                    return depthCustomValue;

                case Depth.SortingOrder:

                    var sortDepth = mainShape.spriteShape.GetSortingOrder();

                    sortDepth = Mathf.Max(Mathf.Min(sortDepth, 100), -100);

                    return sortDepth;

                case Depth.ZPosition:

                    var zDepth = (int)transform.position.z;

                    zDepth = Mathf.Max(Mathf.Min(zDepth, 100), -100);

                    return zDepth;

                case Depth.YPosition:

                    var yDepth = (int)transform.position.y;

                    zDepth = Mathf.Max(Mathf.Min(yDepth, 100), -100);

                    return zDepth;
            }

            return 0;
        }

        public bool InAnyCamera() // camera transform
        {
            var lightingCameras = CameraTransform.List;

            // Rect lightRect = transform2D.WorldRect;

            for (var i = 0; i < lightingCameras.Count; i++)
            {
                var cameraTransform = lightingCameras[i];

                var camera = cameraTransform.Camera;

                if (camera == null) continue;

                var distance = Vector2.Distance(transform.position, camera.transform.position);
                var cameraRadius = CameraTransform.GetRadius(camera);

                // 5 = size
                // why not using rect overlap?
                var radius = cameraRadius + 5;

                if (distance < radius) return true;
            }

            return false;
        }

        public static void ForceUpdateAll()
        {
            foreach (var collider in List) collider.ForceUpdate();
        }

        public void ForceUpdate()
        {
            Initialize();

            mainShape.transform2D.updateNeeded = true;
        }

        public void UpdateLoop()
        {
            if (isStatic) return;

            mainShape.transform2D.Update();

            // ???

            if (mainShape.transform2D.updateNeeded) mainShape.transform2D.updateNeeded = false;
        }

        public void Initialize()
        {
            mainShape.shadowType = shadowType;
            mainShape.thickness = shadowThickness;
            mainShape.maskType = maskType;
            mainShape.height = shadowDistance;

            mainShape.isStatic = isStatic;

            mainShape.SetTransform(transform);
            mainShape.ResetLocal();

            mainShape.transform2D.Update();
        }

        private void DrawGizmos()
        {
            if (mainShape.shadowType != ShadowType.None)
            {
                Gizmos.color = new Color(1f, 0.5f, 0.25f);

                switch (mainShape.shadowType)
                {
                    case ShadowType.SpriteProjection:

                        Vector2 pos = transform.position;
                        var rot = Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;

                        var pair = Pair2.Zero();

                        pair.A = pos + pair.A.Push(-rot + Mathf.PI / 2, shadowThickness);

                        pair.B = pos + pair.B.Push(-rot - Mathf.PI / 2, shadowThickness);

                        Gizmos.DrawLine(pair.A, pair.B);

                        break;

                    case ShadowType.Collider2D:
                    case ShadowType.SpritePhysicsShape:
                    case ShadowType.SpriteProjectionShape:
                    case ShadowType.SpriteProjectionCollider:
                    case ShadowType.FillCollider2D:
                    case ShadowType.FillSpritePhysicsShape:

                        var polygons = mainShape.GetPolygonsWorld();

                        if (polygons != null) GizmosHelper.DrawPolygons(polygons, transform.position);

                        if (mainShape.shadowType == ShadowType.SpriteProjectionShape)
                        {
                            var direcion = Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;

                            foreach (var polygon in polygons)
                            {
                                var axis = Polygon2Helper.GetAxis(polygon, direcion);

                                Gizmos.DrawLine(axis.A, axis.B);
                            }
                        }

                        break;
                }

                switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
                {
                    case EditorGizmosBounds.Enabled:

                        Gizmos.color = new Color(0, 1f, 1f, 0.25f);

                        switch (mainShape.shadowType)
                        {
                            case ShadowType.Collider2D:
                            case ShadowType.SpritePhysicsShape:

                                var bound = mainShape.GetShadowBounds();
                                GizmosHelper.DrawRect(transform.position, bound);

                                break;
                        }

                        break;
                }
            }
        }
    }
}