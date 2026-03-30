using System.Collections.Generic;
using FunkyCode.LightingSettings;
using UnityEngine;
using UnityEngine.Rendering;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class OnRenderMode : LightingMonoBehaviour
    {
        public static List<OnRenderMode> List = new();
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public LightMainBuffer2D mainBuffer;

        private void Update()
        {
            if (mainBuffer == null || !mainBuffer.IsActive)
            {
                DestroySelf();
                return;
            }

            if (mainBuffer.cameraSettings.GetCamera() == null)
            {
                DestroySelf();
                return;
            }

            if (Lighting2D.RenderingMode != RenderingMode.OnRender) DestroySelf();
        }

        public void OnEnable()
        {
            List.Add(this);
        }

        public void OnDisable()
        {
            List.Remove(this);
        }

        public static OnRenderMode Get(LightMainBuffer2D buffer)
        {
            foreach (var meshModeObject in List)
                if (meshModeObject.mainBuffer == buffer)
                    return meshModeObject;

            var meshRendererMode = new GameObject("On Render");
            var onRenderMode = meshRendererMode.AddComponent<OnRenderMode>();

            onRenderMode.mainBuffer = buffer;
            onRenderMode.Initialize(buffer);
            onRenderMode.UpdateLayer();

            if (Lighting2D.ProjectSettings.managerInternal == ManagerInternal.HideInHierarchy)
                meshRendererMode.hideFlags = meshRendererMode.hideFlags | HideFlags.HideInHierarchy;

            onRenderMode.name = "On Render: " + buffer.name;

            return onRenderMode;
        }

        public void Initialize(LightMainBuffer2D mainBuffer)
        {
            if (mainBuffer == null) Debug.Log("main buffer null");

            gameObject.transform.parent = LightingManager2D.Get().transform;

            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = mainBuffer.GetMaterial();
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.allowOcclusionWhenDynamic = false;

            var lightmapPreset = mainBuffer.GetLightmapPreset();

            if (lightmapPreset != null)
                mainBuffer.cameraLightmap.sortingLayer.ApplyToMeshRenderer(meshRenderer);
            else
                Debug.Log("light preset null");

            UpdatePosition();

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = LightingRender2D.GetMesh();
        }

        public void UpdateLoop()
        {
            if (mainBuffer == null || !mainBuffer.IsActive) return;

            if (mainBuffer.cameraSettings.GetCamera() == null) return;

            if (Lighting2D.RenderingMode != RenderingMode.OnRender) return;

            UpdateLayer();

            if (Lighting2D.Disable)
                if (meshRenderer != null)
                    meshRenderer.enabled = false;

            if (mainBuffer.cameraLightmap.overlay != CameraLightmap.Overlay.Enabled) meshRenderer.enabled = false;

            if (mainBuffer.cameraLightmap.rendering != CameraLightmap.Rendering.Enabled) meshRenderer.enabled = false;

            if (Lighting2D.RenderingMode == RenderingMode.OnRender) UpdatePosition();
        }

        private void UpdateLayer()
        {
            gameObject.layer =
                mainBuffer != null ? mainBuffer.cameraSettings.GetLayerId(mainBuffer.cameraLightmap.id) : 0;
        }

        public void UpdatePosition()
        {
            var camera = mainBuffer.cameraSettings.GetCamera();

            if (camera == null) return;

            switch (mainBuffer.cameraLightmap.overlayPosition)
            {
                case CameraLightmap.OverlayPosition.Camera:

                    transform.position = LightingPosition.GetCameraPlanePosition(camera);

                    break;

                case CameraLightmap.OverlayPosition.Custom:

                    transform.position =
                        LightingPosition.GetCameraCustomPosition(camera, mainBuffer.cameraLightmap.customPosition);

                    break;
            }

            transform.rotation = camera.transform.rotation;

            transform.localScale = LightingRender2D.GetSize(camera);
        }
    }
}