using FunkyCode.Rendering.Manager;

//#if UNITY_EDITOR
//	using UnityEditor;
//#endif

namespace FunkyCode
{
    public class SceneView
    {
#if UNITY_EDITOR
        private void OnSceneView(UnityEditor.SceneView sceneView)
        {
            var manager = LightingManager2D.Get();

            if (!IsSceneViewActive()) return;

            Main.InternalUpdate();

            Main.Render();
        }
#endif

        public void OnDisable()
        {
#if UNITY_EDITOR

#if UNITY_2019_1_OR_NEWER

            UnityEditor.SceneView.beforeSceneGui -= OnSceneView;
            //SceneView.duringSceneGui -= OnSceneView;

#else
					UnityEditor.SceneView.onSceneGUIDelegate -= OnSceneView;

#endif

#endif
        }

        public void OnEnable()
        {
#if UNITY_EDITOR

#if UNITY_2019_1_OR_NEWER

            UnityEditor.SceneView.beforeSceneGui += OnSceneView;
            //SceneView.duringSceneGui += OnSceneView;

#else
					UnityEditor.SceneView.onSceneGUIDelegate += OnSceneView;

#endif
#endif
        }

        public bool IsSceneViewActive() // overlay
        {
            var manager = LightingManager2D.Get();

            for (var i = 0; i < manager.cameras.Length; i++)
            {
                var cameraSetting = manager.cameras.Get(i);

                for (var b = 0; b < cameraSetting.Lightmaps.Length; b++)
                {
                    var cameraLightmap = cameraSetting.GetLightmap(b);

                    if (cameraLightmap.sceneView == CameraLightmap.SceneView.Enabled) return true;
                }
            }

            return false;
        }
    }
}