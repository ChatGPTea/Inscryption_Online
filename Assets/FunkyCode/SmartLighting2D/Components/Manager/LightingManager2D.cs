using FunkyCode.LightingSettings;
using FunkyCode.Rendering;
using FunkyCode.Rendering.Manager;
using UnityEngine;
using Gizmos = UnityEngine.Gizmos;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightingManager2D : LightingMonoBehaviour
    {
        private static LightingManager2D instance;

        // Sets Lighting Main Profile Settings for Lighting2D at the start of the scene
        private static bool initialized;

        [SerializeField] public LightingCameras cameras = new();

        public int version;
        public string version_string = "";

        public Profile setProfile;
        public Profile profile;

        // editor foldouts (avoid reseting after compiling script)
        public bool[] foldout_cameras = new bool[10];
        public bool[,] foldout_lightmapMaterials = new bool[10, 10];

        public bool[,] foldout_lightmapPresets = new bool[10, 10];

        private readonly SceneView sceneView = new();

        public void Awake()
        {
            if (cameras == null) cameras = new LightingCameras();

            if (Application.isPlaying) Lighting2D.ProjectSettings.shaderPreview = ShaderPreview.Disabled;

            CameraTransform.List.Clear();

            if (instance != null && instance != this)
                switch (Lighting2D.ProjectSettings.managerInstance)
                {
                    case ManagerInstance.Static:
                    case ManagerInstance.DontDestroyOnLoad:

                        Debug.LogWarning(
                            "Smart Lighting2D: Lighting Manager duplicate was found, new instance destroyed.",
                            gameObject);

                        foreach (var manager in FindObjectsOfType<LightingManager2D>())
                            if (manager != instance)
                                manager.DestroySelf();

                        return; // cancel initialization

                    case ManagerInstance.Dynamic:

                        instance = this;

                        Debug.LogWarning(
                            "Smart Lighting2D: Lighting Manager duplicate was found, old instance destroyed.",
                            gameObject);

                        foreach (var manager in FindObjectsOfType<LightingManager2D>())
                            if (manager != instance)
                                manager.DestroySelf();

                        break;
                }

            initialized = false;

            SetupProfile();

            if (Application.isPlaying)
                if (Lighting2D.ProjectSettings.managerInstance == ManagerInstance.DontDestroyOnLoad)
                    DontDestroyOnLoad(instance.gameObject);
        }

        // use only late update?
        private void Update()
        {
            if (Lighting2D.Disable) return;

            ForceUpdate(); // for late update method?

            if (profile != null)
                if (Lighting2D.Profile != profile)
                    Lighting2D.UpdateByProfile(profile);

            FixTransform();
        }

        private void LateUpdate()
        {
            if (Lighting2D.Disable) return;

            UpdateInternal();

            if (Lighting2D.Profile.qualitySettings.updateMethod == UpdateMethod.LateUpdate) Main.Render();
        }

        private void OnEnable()
        {
            sceneView.OnEnable();

            foreach (var onRenderMode in FindObjectsOfType<OnRenderMode>()) onRenderMode.DestroySelf();

            Scriptable.LightSprite2D.List.Clear();

            UpdateProfile();

            Main.UpdateMaterials();

            Update();
            LateUpdate();
        }

        private void OnDisable()
        {
            sceneView.OnDisable();

            if (profile == null) return;

            if (Application.isPlaying)
                if (setProfile != profile)
                    if (Lighting2D.Profile == profile)
                        Lighting2D.RemoveProfile();
        }

        private void OnDestroy()
        {
            LightmapShaders.ResetShaders();
        }

        private void OnDrawGizmos()
        {
            if (Lighting2D.ProjectSettings.gizmos.drawGizmos != EditorDrawGizmos.Always) return;

            DrawGizmos();
        }

        private void OnRenderObject()
        {
            if (Lighting2D.RenderingMode != RenderingMode.OnPostRender) return;

            foreach (var buffer in LightMainBuffer2D.List) LightMainBuffer.DrawPost(buffer);
        }

        public Camera GetCamera(int id)
        {
            if (cameras.Length <= id) return null;

            return cameras.Get(id).GetCamera();
        }

        public static void ForceUpdate()
        {
        }

        public static LightingManager2D Get()
        {
            if (instance != null) return instance;

            foreach (var manager in FindObjectsOfType<LightingManager2D>())
            {
                instance = manager;

                return instance;
            }

            // create new light manager
            var gameObject = new GameObject("Lighting Manager 2D");

            instance = gameObject.AddComponent<LightingManager2D>();
            instance.transform.position = Vector3.zero;
            instance.version = Lighting2D.VERSION;
            instance.version_string = Lighting2D.VERSION_STRING;

            return instance;
        }

        public void FixTransform()
        {
            if (transform.lossyScale != Vector3.one)
            {
                var scale = Vector3.one;

                var parent = transform.parent;

                if (parent != null)
                {
                    scale.x /= parent.lossyScale.x;
                    scale.y /= parent.lossyScale.y;
                    scale.z /= parent.lossyScale.z;
                }

                transform.localScale = Vector3.one;
            }

            if (transform.position != Vector3.one) transform.position = Vector3.zero;

            if (transform.rotation != Quaternion.identity) transform.rotation = Quaternion.identity;
        }

        public void SetupProfile()
        {
            if (initialized) return;

            initialized = true;

            var profile = Lighting2D.Profile;

            Lighting2D.UpdateByProfile(profile);

            Lighting2D.Materials.Reset();
        }

        public void UpdateInternal()
        {
            if (Lighting2D.Disable) return;

            SetupProfile();

            Main.InternalUpdate();
        }

        public void UpdateProfile()
        {
            if (setProfile == null) setProfile = Lighting2D.ProjectSettings.Profile;

            if (Application.isPlaying)
                profile = Instantiate(setProfile);
            else
                profile = setProfile;
        }

        private void DrawGizmos()
        {
            if (!isActiveAndEnabled) return;

            Gizmos.color = new Color(0, 1f, 1f);

            if (Lighting2D.ProjectSettings.gizmos.drawGizmosBounds == EditorGizmosBounds.Enabled)
                for (var i = 0; i < cameras.Length; i++)
                {
                    var cameraSetting = cameras.Get(i);
                    var camera = cameraSetting.GetCamera();
                    if (camera)
                    {
                        var cameraRect = CameraTransform.GetWorldRect(camera);

                        GizmosHelper.DrawRect(transform.position, cameraRect);
                    }
                }

            for (var i = 0; i < Scriptable.LightSprite2D.List.Count; i++)
            {
                var light = Scriptable.LightSprite2D.List[i];
                var rect = light.lightSpriteShape.GetWorldRect();

                Gizmos.color = new Color(1f, 0.5f, 0.25f);
                GizmosHelper.DrawPolygon(light.lightSpriteShape.GetSpriteWorldPolygon(), transform.position);

                Gizmos.color = new Color(0, 1f, 1f);
                GizmosHelper.DrawRect(transform.position, rect);
            }
        }
    }
}