using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FunkyCode
{
    [CustomEditor(typeof(LightRoom2D))]
    public class LightRoom2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as LightRoom2D;

            script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer,
                Lighting2D.Profile.layers.lightLayers.GetNames());

            script.shape.type = (LightRoom2D.RoomType)EditorGUILayout.EnumPopup("Room Type", script.shape.type);

            script.color = EditorGUILayout.ColorField("Color", script.color);

            Update();

            if (GUI.changed)
                if (!EditorApplication.isPlaying)
                {
                    EditorUtility.SetDirty(script);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

                    LightingManager2D.ForceUpdate();
                }
        }

        private void Update()
        {
            var script = target as LightRoom2D;

            if (GUILayout.Button("Update")) script.Initialize();
        }
    }
}