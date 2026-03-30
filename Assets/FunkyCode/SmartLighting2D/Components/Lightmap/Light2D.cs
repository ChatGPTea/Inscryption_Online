using System;
using System.Collections.Generic;
using FunkyCode.Buffers;
using FunkyCode.EventHandling;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;
using Object = FunkyCode.EventHandling.Object;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class Light2D : LightingMonoBehaviour
    {
        public enum LightSprite
        {
            Default,
            Custom
        }

        public enum LightType
        {
            Point,
            Sprite,
            FreeForm
        }

        public enum LitMode
        {
            Everything,
            MaskOnly
        }

        public enum MaskTranslucencyQuality
        {
            Disabled,
            LowQuality,
            MediumQuality,
            HighQuality
        }

        public enum Rotation
        {
            Disabled,
            World,
            Local
        }

        public enum WhenInsideCollider
        {
            Draw,
            DontDraw,
            Ignore
        }

        public static List<Light2D> List = new();
        private static Sprite defaultSprite;

        public LightType lightType = LightType.Sprite;

        // settings
        public int lightPresetId;
        public int eventPresetId;

        // light layer
        public int lightLayer;
        public int occlusionLayer;
        public int translucentLayer;

        public int translucentPresetId;

        public Color color = new(.5f, .5f, .5f, 1);

        public float size = 5f;

        public float spotAngleInner = 360;
        public float spotAngleOuter = 360;

        // soft shadow
        public float coreSize = 0.5f;
        public float falloff;

        public float lightStrength;

        // legacy shadow
        public float outerAngle = 15;

        public float lightRadius = 1;

        public float shadowDistanceClose;
        public float shadowDistanceFar = 5;

        public MaskTranslucencyQuality maskTranslucencyQuality = MaskTranslucencyQuality.LowQuality;
        public float maskTranslucencyStrength = 0.5f;

        public Rotation applyRotation = Rotation.Disabled;

        public LightingSourceTextureSize textureSize = LightingSourceTextureSize.px2048;

        public MeshMode meshMode = new();
        public BumpMap bumpMap = new();

        public WhenInsideCollider whenInsideCollider = WhenInsideCollider.Draw;

        public LightSprite lightSprite = LightSprite.Default;
        public Sprite sprite;
        public bool spriteFlipX;
        public bool spriteFlipY;
        public float freeFormFalloff = 1;
        public float freeFormPoint = 1;
        public float freeFormFalloffStrength = 1;
        public FreeFormPoints freeFormPoints = new();

        public LightEventHandling eventHandling = new();
        public bool drawingEnabled;
        public bool drawingTranslucencyEnabled;

        // Internal
        private readonly List<LightCollider2D> collidersInside = new();
        private readonly List<LightCollider2D> collidersInsideRemove = new();

        public LightFreeForm freeForm;
        private bool inScreen;

        public LightTransform transform2D;

        public LightBuffer2D Buffer { get; set; }

        public void OnEnable()
        {
            List.Add(this);

            if (transform2D == null) transform2D = new LightTransform();

            if (freeForm == null) freeForm = new LightFreeForm();

            LightingManager2D.Get();

            collidersInside.Clear();

            ForceUpdate();
        }

        public void OnDisable()
        {
            List.Remove(this);

            Free();
        }

        private void OnDrawGizmos()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos == EditorDrawGizmos.Disabled) return;

            if (Lighting2D.ProjectSettings.gizmos.drawIcons == EditorIcons.Enabled)
                Gizmos.DrawIcon(transform.position, "light_v2", true);

            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Always) return;

            Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Selected) return;

            Draw();
        }

        public void AddEvent(CollisionEvent2D collisionEvent)
        {
            eventHandling.eventHandlingObject.collisionEvents += collisionEvent;
        }

        public void AddCollider(LightCollider2D id)
        {
            if (collidersInside.Contains(id))
            {
                if (lightPresetId > 0) id.lightOnEnter?.Invoke(this);

                collidersInside.Add(id);
            }
        }

        public LayerSetting[] GetLightPresetLayers()
        {
            var presetList = Lighting2D.Profile.lightPresets;

            if (lightPresetId >= presetList.list.Length) return null;

            var lightPreset = presetList.Get()[lightPresetId];

            return lightPreset.layerSetting.Get();
        }

        public LayerSetting[] GetTranslucencyPresetLayers()
        {
            var presetList = Lighting2D.Profile.lightPresets;

            if (translucentPresetId >= presetList.list.Length) return null;

            var lightPreset = presetList.Get()[translucentPresetId];

            return lightPreset.layerSetting.Get();
        }

        public EventPreset GetEventPreset()
        {
            var presetList = Lighting2D.Profile.eventPresets;

            if (eventPresetId >= presetList.list.Length) return null;

            var lightPreset = presetList.Get()[eventPresetId];

            return lightPreset;
        }

        public static Sprite GetDefaultSprite()
        {
            if (!defaultSprite || !defaultSprite.texture) defaultSprite = Resources.Load<Sprite>("Sprites/gfx_light");

            return defaultSprite;
        }

        public Sprite GetSprite()
        {
            if (!sprite || !sprite.texture) sprite = GetDefaultSprite();

            return sprite;
        }

        public void ForceUpdate()
        {
            if (transform2D == null) return;

            transform2D.ForceUpdate();

            freeForm.ForceUpdate();
        }

        public static void ForceUpdateAll()
        {
            foreach (var light in List) light.ForceUpdate();
        }

        public void Free()
        {
            Manager.FreeBuffer(Buffer);

            inScreen = false;
        }

        // used to check if camera is used in the system

        public bool InCameras()
        {
            var lightingCameras = CameraTransform.List;
            var lightRect = transform2D.WorldRect;

            for (var i = 0; i < lightingCameras.Count; i++)
            {
                var cameraTransform = lightingCameras[i];
                var camera = cameraTransform.Camera;
                if (!camera)
                    continue;

                var cameraRect = cameraTransform.WorldRect();
                if (cameraRect.Overlaps(lightRect)) return true;
            }

            return false;
        }

        // to check if light is rendered for specific lightmap

        public bool InCamera(Camera camera)
        {
            var lightRect = transform2D.WorldRect;
            var cameraRect = CameraTransform.GetWorldRect(camera);

            return cameraRect.Overlaps(lightRect);
        }

        // light 2D should know what layers id's it is supposed to draw? (include in array)

        public bool IfDrawLightCollider(LightCollider2D lightCollider)
        {
            var layerSetting = GetLightPresetLayers();
            if (layerSetting == null) return false;

            for (var i = 0; i < layerSetting.Length; i++)
            {
                var setting = layerSetting[i];
                if (setting == null)
                    continue;

                var layerID = setting.layerID;
                switch (setting.type)
                {
                    case LightLayerType.ShadowAndMask:

                        if (layerID == lightCollider.shadowLayer || layerID == lightCollider.maskLayer) return true;
                        break;

                    case LightLayerType.MaskOnly:

                        if (layerID == lightCollider.maskLayer) return true;
                        break;

                    case LightLayerType.ShadowOnly:

                        if (layerID == lightCollider.shadowLayer) return true;
                        break;
                }
            }

            return false;
        }

        public Vector2Int GetTextureSize()
        {
            var textureSize2D = LightingRender2D.GetTextureSize(textureSize);

            if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSourceTextureSize.Custom)
                textureSize2D = LightingRender2D.GetTextureSize(Lighting2D.Profile.qualitySettings.lightTextureSize);

            return textureSize2D;
        }

        public bool IsPixelPerfect()
        {
            if (Lighting2D.Profile.qualitySettings.lightTextureSize != LightingSourceTextureSize.Custom)
                return Lighting2D.Profile.qualitySettings.lightTextureSize == LightingSourceTextureSize.PixelPerfect;

            return textureSize == LightingSourceTextureSize.PixelPerfect;
        }

        public LightBuffer2D GetBuffer()
        {
            if (Buffer == null) Buffer = Manager.PullBuffer(this);

            return Buffer;
        }

        public void UpdateLoop()
        {
            transform2D.Update(this);

            if (lightType == LightType.FreeForm) freeForm.Update(this);

            // if camera moves & pixel perfect
            if (IsPixelPerfect()) transform2D.ForceUpdate();

            UpdateBuffer();

            DrawMeshMode();

            if (eventPresetId > 0)
            {
                var eventPreset = GetEventPreset();
                if (eventPreset != null) eventHandling.eventHandlingObject.Update(this, eventPreset);
            }
        }

        private void BufferUpdate()
        {
            transform2D.ClearUpdate();

            if (Lighting2D.Disable) return;

            if (Buffer == null)
                return;

            Buffer.updateNeeded = true;
        }

        private void UpdateCollidersInside()
        {
            foreach (var collider in collidersInside)
            {
                if (collider == null)
                {
                    collidersInsideRemove.Add(collider);
                    continue;
                }

                if (!collider.isActiveAndEnabled)
                {
                    collidersInsideRemove.Add(collider);
                    continue;
                }

                if (!collider.InLight(this)) collidersInsideRemove.Add(collider);
            }

            foreach (var collider in collidersInsideRemove)
            {
                collidersInside.Remove(collider);
                transform2D.ForceUpdate();

                if (eventPresetId > 0)
                    if (collider)
                        collider.lightOnExit?.Invoke(this);
            }

            collidersInsideRemove.Clear();
        }

        private void UpdateBuffer()
        {
            UpdateCollidersInside();

            if (InCameras())
            {
                if (GetBuffer() == null) return;

                if (transform2D.UpdateNeeded || !inScreen)
                {
                    BufferUpdate();

                    inScreen = true;
                }
            }
            else
            {
                if (Buffer != null) Manager.FreeBuffer(Buffer);

                inScreen = false;
            }
        }

        public void DrawMeshMode()
        {
            if (!meshMode.enable) return;

            if (Buffer == null) return;

            if (!isActiveAndEnabled) return;

            if (!InCameras()) return;

            var lightingMesh = MeshRendererManager.Pull(this);
            if (lightingMesh) lightingMesh.UpdateLight(this, meshMode);
        }

        private void Draw()
        {
            if (!isActiveAndEnabled) return;

            Gizmos.color = new Color(1f, 0.5f, 0.25f);

            if (applyRotation != Rotation.Disabled)
                GizmosHelper.DrawCircle(transform.position, transform2D.rotation, 360, size); // spotAngle
            else
                GizmosHelper.DrawCircle(transform.position, 0, 360, size); // spotAngle

            Gizmos.color = new Color(0, 1f, 1f);

            switch (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds)
            {
                case EditorGizmosBounds.Enabled:

                    GizmosHelper.DrawRect(transform.position, transform2D.WorldRect);

                    break;
            }
        }

        [Serializable]
        public class LightEventHandling
        {
            public Object eventHandlingObject = new();
        }

        [Serializable]
        public class BumpMap
        {
            public float intensity = 1;
            public float depth = 1;
        }
    }
}