using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FunkyCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightTilemapOcclusion2D))]
    public class LightTilemap2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as LightTilemapOcclusion2D;

            script.tilemapType =
                (LightTilemapOcclusion2D.MapType)EditorGUILayout.EnumPopup("Tilemap Type", script.tilemapType);

            script.onlyColliders = EditorGUILayout.Toggle("Only Colliders", script.onlyColliders);

            GUISortingLayer.Draw(script.sortingLayer, false);

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