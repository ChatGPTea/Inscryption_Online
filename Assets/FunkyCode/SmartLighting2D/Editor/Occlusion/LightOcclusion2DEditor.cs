using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FunkyCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightOcclusion2D))]
    public class LightOcclusion2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as LightOcclusion2D;

            script.shape.shadowType =
                (LightOcclusion2D.ShadowType)EditorGUILayout.EnumPopup("Shadow Type", script.shape.shadowType);

            script.occlusionType =
                (LightOcclusion2D.OcclusionType)EditorGUILayout.EnumPopup("Occlusion Type", script.occlusionType);

            script.occlusionSize = EditorGUILayout.FloatField("Size", script.occlusionSize);

            if (GUILayout.Button("Update")) script.Initialize();

            if (GUI.changed)
            {
                script.Initialize();

                if (!EditorApplication.isPlaying)
                {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }
    }
}