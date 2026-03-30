using System.Collections.Generic;
using FunkyCode.EventHandling;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEngine;
using UnityEngine.Events;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightCollider2D : MonoBehaviour
    {
        public enum MaskPivot
        {
            TransformCenter,
            ShapeCenter,
            LowestY
        }

        public enum MaskType
        {
            None,
            Sprite,
            BumpedSprite,
            SpritePhysicsShape,
            CompositeCollider2D,
            Collider2D,
            Collider3D,
            MeshRenderer,
            BumpedMeshRenderer,
            SkinnedMeshRenderer
        }

        public enum ShadowDistance
        {
            Infinite,
            Finite
        }

        public enum ShadowType
        {
            None,
            SpritePhysicsShape,
            CompositeCollider2D,
            Collider2D,
            Collider3D,
            MeshRenderer,
            SkinnedMeshRenderer
        }

        public static List<LightCollider2D> List = new();
        public static List<LightCollider2D> ListEventReceivers = new();
        public static LightColliderLayer<LightCollider2D> layerManagerMask = new();
        public static LightColliderLayer<LightCollider2D> layerManagerShadow = new();

        // shadow
        public ShadowType shadowType = ShadowType.SpritePhysicsShape;
        public int shadowLayer;

        [Min(0)] public ShadowDistance shadowDistance = ShadowDistance.Infinite;

        [Min(0.1f)] public float shadowDistanceMin = 0.5f;

        [Min(0)] public float shadowDistanceMax = 1f;

        [Range(0, 1)] public float shadowTranslucency;

        // mask
        public MaskType maskType = MaskType.None;
        public MaskLit maskLit = MaskLit.Lit;
        public MaskPivot maskPivot = MaskPivot.TransformCenter;
        public int maskLayer;

        [Range(0, 1)] public float maskLitCustom = 1;

        public BumpMapMode bumpMapMode = new();

        // internal

        public LightColliderShape mainShape = new();
        public LightEvent lightOnEnter;
        public LightEvent lightOnExit;

        // list manager 
        private int listMaskLayer = -1;
        private int listShadowLayer = -1;

        public SpriteMeshObject spriteMeshObject = new();

        public bool isStatic => gameObject.isStatic;

        private void OnEnable()
        {
            List.Add(this);

            UpdateLayerList();

            LightingManager2D.Get();

            Initialize();

            UpdateNearbyLights();

            bumpMapMode.SetSpriteRenderer(mainShape.spriteShape.GetSpriteRenderer());
        }

        private void OnDisable()
        {
            List.Remove(this);

            ClearLayerList();

            UpdateNearbyLights();
        }

        private void OnDestroy()
        {
            List.Remove(this);

            if (ListEventReceivers.Count > 0)
                if (ListEventReceivers.Contains(this))
                    ListEventReceivers.Remove(this);
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

        // event handling

        public event CollisionEvent2D collisionEvents;

        public bool ShadowDisabled()
        {
            return mainShape.shadowType == ShadowType.None;
        }

        public void AddEventOnEnter(UnityAction<Light2D> call)
        {
            if (lightOnEnter == null) lightOnEnter = new LightEvent();

            lightOnEnter.AddListener(call);
        }

        public void AddEventOnExit(UnityAction<Light2D> call)
        {
            if (lightOnExit == null) lightOnExit = new LightEvent();

            lightOnExit.AddListener(call);
        }

        public void AddEvent(CollisionEvent2D collisionEvent)
        {
            collisionEvents += collisionEvent;

            ListEventReceivers.Add(this);
        }

        public void RemoveEvent(CollisionEvent2D collisionEvent)
        {
            ListEventReceivers.Remove(this);

            collisionEvent -= collisionEvent;
        }

        public static void ForceUpdateAll()
        {
            foreach (var lightCollider2D in List) lightCollider2D.Initialize();
        }

        // Layer List
        private void ClearLayerList()
        {
            layerManagerMask.Remove(listMaskLayer, this);
            layerManagerShadow.Remove(listShadowLayer, this);

            listMaskLayer = -1;
            listShadowLayer = -1;
        }

        private void UpdateLayerList()
        {
            listMaskLayer = layerManagerMask.Update(listMaskLayer, maskLayer, this);
            listShadowLayer = layerManagerShadow.Update(listShadowLayer, shadowLayer, this);
        }

        public static List<LightCollider2D> GetMaskList(int layer)
        {
            return layerManagerMask.layerList[layer];
        }

        public static List<LightCollider2D> GetShadowList(int layer)
        {
            return layerManagerShadow.layerList[layer];
        }

        public void CollisionEvent(LightCollision2D collision)
        {
            collisionEvents?.Invoke(collision);
        }

        public bool InLight(Light2D light)
        {
            return mainShape.RectOverlap(light.transform2D.WorldRect);
        }

        // light 2D method?
        // light 2D should know what layers id's it is supposed to draw? (include in array)

        public void UpdateNearbyLights()
        {
            for (var id = 0; id < Light2D.List.Count; id++)
            {
                var light = Light2D.List[id];

                if (!light.IfDrawLightCollider(this)) continue;

                if (InLight(light)) light.ForceUpdate();
            }
        }

        public void Initialize()
        {
            mainShape.maskType = maskType;
            mainShape.maskPivot = maskPivot;

            mainShape.shadowType = shadowType;

            mainShape.SetTransform(this);
            mainShape.transform2D.Reset();
            mainShape.transform2D.Update(true);
            mainShape.transform2D.UpdateNeeded = true;

            mainShape.ResetLocal();
        }

        public void UpdateLoop()
        {
            UpdateLayerList();

            if (isStatic) return;

            var updateLights = false;

            mainShape.transform2D.Update(false);

            if (mainShape.transform2D.UpdateNeeded)
            {
                mainShape.transform2D.UpdateNeeded = false;

                mainShape.ResetWorld();

                updateLights = true;
            }

            if (updateLights) UpdateNearbyLights();
        }

        private void DrawGizmos()
        {
            if (!isActiveAndEnabled) return;

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosShadowCasters)
            {
                case EditorShadowCasters.Enabled:

                    Gizmos.color = new Color(1f, 0.5f, 0.25f);

                    if (mainShape.shadowType != ShadowType.None)
                    {
                        var polygons = mainShape.GetPolygonsWorld();

                        GizmosHelper.DrawPolygons(polygons, transform.position);
                    }

                    break;
            }

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    if (maskLit == MaskLit.Isometric)
                    {
                        Gizmos.color = Color.green;
                        GizmosHelper.DrawIsoRect(transform.position, mainShape.GetIsoWorldRect());
                    }
                    else
                    {
                        Gizmos.color = new Color(0, 1f, 1f, 0.5f);
                        GizmosHelper.DrawRect(transform.position, mainShape.GetWorldRect());
                    }

                    break;
            }

            if (Lighting2D.ProjectSettings.gizmos.drawIcons == EditorIcons.Enabled)
            {
                Vector2? pivotPoint = mainShape.GetPivotPoint();

                if (pivotPoint != null)
                {
                    var pos = transform.position;
                    pos.x = pivotPoint.Value.x;
                    pos.y = pivotPoint.Value.y;

                    Gizmos.DrawIcon(pos, "circle_v2", true);
                }
            }
        }
    }
}