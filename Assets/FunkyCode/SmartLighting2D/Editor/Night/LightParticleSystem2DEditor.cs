using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FunkyCode
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightParticleSystem2D))]
    public class LightParticleSystem2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var script = target as LightParticleSystem2D;

            script.lightLayer = EditorGUILayout.Popup("Layer (Light)", script.lightLayer,
                Lighting2D.Profile.layers.lightLayers.GetNames());

            script.color = EditorGUILayout.ColorField("Color", script.color);

            script.color.a = EditorGUILayout.Slider("Alpha", script.color.a, 0, 1);

            script.scale = EditorGUILayout.FloatField("Scale", script.scale);

            script.useParticleColor = EditorGUILayout.Toggle("Use Particle Color", script.useParticleColor);

            script.customParticle =
                (Texture)EditorGUILayout.ObjectField("Custom Particle", script.customParticle, typeof(Texture), true);

            if (GUI.changed)
                if (!EditorApplication.isPlaying)
                {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
        }
    }
}