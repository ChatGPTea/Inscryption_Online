using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode
{
    public class EditorGameObjects : MonoBehaviour
    {
        public static Camera GetCamera()
        {
            Camera camera = null;

            if (UnityEditor.SceneView.lastActiveSceneView != null &&
                UnityEditor.SceneView.lastActiveSceneView.camera != null)
                camera = UnityEditor.SceneView.lastActiveSceneView.camera;
            else if (Camera.main != null) camera = Camera.main;

            return camera;
        }

        public static Vector3 GetCameraPoint()
        {
            var pos = Vector3.zero;

            var camera = GetCamera();

            if (camera != null)
            {
                var worldRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
                pos = worldRay.origin;
                pos.z = 0;
            }
            else
            {
                Debug.LogError("Scene Camera Not Found");
            }

            return pos;
        }

        [MenuItem("GameObject/2D Light/Light/Light", false, 4)]
        private static void CreateLightSource()
        {
            var newGameObject = new GameObject("Light 2D");

            newGameObject.AddComponent<Light2D>();

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Collider/Light Collider", false, 4)]
        private static void CreateLightCollider()
        {
            var newGameObject = new GameObject("Light Collider 2D");

            newGameObject.AddComponent<PolygonCollider2D>();
            var collider = newGameObject.AddComponent<LightCollider2D>();
            collider.maskType = LightCollider2D.MaskType.Collider2D;
            collider.shadowType = LightCollider2D.ShadowType.Collider2D;
            collider.Initialize();

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Collider/Light Tilemap Collider", false, 4)]
        private static void CreateLightTilemapCollider()
        {
            var newGrid = new GameObject("2D Light Grid");
            newGrid.AddComponent<Grid>();

            var newGameObject = new GameObject("2D Light Tilemap");
            newGameObject.transform.parent = newGrid.transform;

            newGameObject.AddComponent<Tilemap>();
            newGameObject.AddComponent<LightTilemapCollider2D>();
        }

        [MenuItem("GameObject/2D Light/Light/Light Sprite", false, 4)]
        private static void CreateLightSpriteRenderer()
        {
            var newGameObject = new GameObject("Light Sprite 2D");

            var spriteRenderer2D = newGameObject.AddComponent<LightSprite2D>();
            spriteRenderer2D.sprite = Resources.Load<Sprite>("Sprites/gfx_light");

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Light/Light Texture", false, 4)]
        private static void CreateLightTextureRenderer()
        {
            var newGameObject = new GameObject("Light Texture 2D ");

            var textureRenderer = newGameObject.AddComponent<LightTexture2D>();
            textureRenderer.texture = Resources.Load<Texture>("Sprites/gfx_light");

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Collider/Day Light Collider", false, 4)]
        private static void CreateDayLightCollider()
        {
            var newGameObject = new GameObject("DayLight Collider 2D");

            newGameObject.AddComponent<PolygonCollider2D>();

            var c = newGameObject.AddComponent<DayLightCollider2D>();
            c.mainShape.shadowType = DayLightCollider2D.ShadowType.Collider2D;
            c.mainShape.maskType = DayLightCollider2D.MaskType.None;

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Collider/Day Light Tilemap Collider", false, 4)]
        private static void CreateDayLightTilemapCollider()
        {
            var newGrid = new GameObject("Light Grid 2D");
            newGrid.AddComponent<Grid>();

            var newGameObject = new GameObject("DayLight Tilemap 2D");
            newGameObject.transform.parent = newGrid.transform;

            newGameObject.AddComponent<Tilemap>();
            newGameObject.AddComponent<DayLightTilemapCollider2D>();
        }

        [MenuItem("GameObject/2D Light/Room/Light Room", false, 4)]
        private static void CreateLightRoom()
        {
            var newGameObject = new GameObject("Light Room 2D");

            newGameObject.AddComponent<PolygonCollider2D>();
            newGameObject.AddComponent<LightRoom2D>();

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Room/Light Tilemap Room", false, 4)]
        private static void CreateLightTilemapRoom()
        {
            var newGrid = new GameObject("2D Light Grid");
            newGrid.AddComponent<Grid>();

            var newGameObject = new GameObject("Light Tilemap Room 2D");
            newGameObject.transform.parent = newGrid.transform;

            newGameObject.AddComponent<Tilemap>();
            newGameObject.AddComponent<LightTilemapRoom2D>();
        }

        [MenuItem("GameObject/2D Light/Occlusion/Light Occlusion", false, 4)]
        private static void CreateLightOcclusion()
        {
            var newGameObject = new GameObject("2D Light Occlusion");

            newGameObject.AddComponent<PolygonCollider2D>();
            newGameObject.AddComponent<LightOcclusion2D>();

            newGameObject.transform.position = GetCameraPoint();
        }

        [MenuItem("GameObject/2D Light/Occlusion/Light Tilemap Occlusion", false, 4)]
        private static void CreateLightTilemapOcclusion()
        {
            var newGrid = new GameObject("Light Grid 2D");
            newGrid.AddComponent<Grid>();

            var newGameObject = new GameObject("Light Tilemap Occlusion 2D");
            newGameObject.transform.parent = newGrid.transform;

            newGameObject.AddComponent<Tilemap>();
            newGameObject.AddComponent<LightTilemapOcclusion2D>();
        }

        [MenuItem("GameObject/2D Light/Light Manager", false, 4)]
        private static void CreateLightManager()
        {
            LightingManager2D.Get();
        }

        [MenuItem("GameObject/2D Light/Light Cycle", false, 4)]
        private static void CreateLightCycle()
        {
            var newGameObject = new GameObject("Light Cycle 2D");

            newGameObject.AddComponent<LightCycle>();

            newGameObject.transform.position = GetCameraPoint();
        }
    }
}