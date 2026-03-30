using System;
using System.Collections.Generic;
using System.Reflection;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using SortingLayer = FunkyCode.LightingSettings.SortingLayer;

namespace FunkyCode
{
    public class GUIFoldout
    {
        private static readonly Dictionary<object, bool> dictionary = new();

        public static bool GetValue(object Object)
        {
            var value = false;

            if (!dictionary.TryGetValue(Object, out value)) dictionary.Add(Object, value);

            return value;
        }

        public static void SetValue(object Object, bool value)
        {
            bool resultVal;

            if (dictionary.TryGetValue(Object, out resultVal))
            {
                dictionary.Remove(Object);
                dictionary.Add(Object, value);
            }
        }

        public static bool Draw(string name, object Object)
        {
            var value = EditorGUILayout.Foldout(GetValue(Object), name, true);

            SetValue(Object, value);

            return value;
        }
    }

    public class GUIFoldoutHeader
    {
        private static readonly Dictionary<object, bool> dictionary = new();

        public static bool GetValue(object Object)
        {
            var value = false;

            if (!dictionary.TryGetValue(Object, out value)) dictionary.Add(Object, value);

            return value;
        }

        public static void SetValue(object Object, bool value)
        {
            bool resultVal;

            if (dictionary.TryGetValue(Object, out resultVal))
            {
                dictionary.Remove(Object);
                dictionary.Add(Object, value);
            }
        }

        public static bool Begin(string name, object Object)
        {
#if UNITY_2019_1_OR_NEWER
            var value = EditorGUILayout.BeginFoldoutHeaderGroup(GetValue(Object), name);
#else
				bool value = EditorGUILayout.Foldout(GetValue(Object), name, true);
#endif

            SetValue(Object, value);

            return value;
        }

        public static void End()
        {
#if UNITY_2019_1_OR_NEWER
            EditorGUILayout.EndFoldoutHeaderGroup();
#endif
        }
    }

    public class GUISortingLayer
    {
        public static string[] GetSortingLayerNames()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var sortingLayersProperty =
                internalEditorUtilityType.GetProperty("sortingLayerNames",
                    BindingFlags.Static | BindingFlags.NonPublic);

            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        public static int[] GetSortingLayerUniqueIDs()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs",
                BindingFlags.Static | BindingFlags.NonPublic);

            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }

        public static void Draw(SerializedObject serializedObject, SortingLayer sortingLayer,
            string serializationDepth = "")
        {
            var order = serializedObject.FindProperty(serializationDepth + "sortingLayer.Order");
            var name = serializedObject.FindProperty(serializationDepth + "sortingLayer.name");


            var sortingLayerNames = GetSortingLayerNames();
            var id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
            var newId = EditorGUILayout.Popup("Sorting Layer", id, sortingLayerNames);

            if (newId > -1 && newId < sortingLayerNames.Length)
            {
                var newName = sortingLayerNames[newId];

                if (newName != sortingLayer.Name) name.stringValue = newName;
            }

            EditorGUILayout.PropertyField(order, new GUIContent("Order in Layer"));
        }

        public static void Draw(SortingLayer sortingLayer, bool drawFoldout)
        {
            if (drawFoldout)
            {
                var value = GUIFoldout.Draw("Sorting Layer", sortingLayer);

                if (!value) return;

                EditorGUI.indentLevel++;
            }

            var sortingLayerNames = GetSortingLayerNames();
            var id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
            var newId = EditorGUILayout.Popup("Sorting Layer", id, sortingLayerNames);

            if (newId > -1 && newId < sortingLayerNames.Length)
            {
                var newName = sortingLayerNames[newId];

                if (newName != sortingLayer.Name) sortingLayer.Name = newName;
            }

            sortingLayer.Order = EditorGUILayout.IntField("Order in Layer", sortingLayer.Order);

            if (drawFoldout) EditorGUI.indentLevel--;
        }
    }

    public class GUIMeshMode
    {
        public static void Draw(SerializedObject serializedObject, MeshMode meshMode)
        {
            var value = GUIFoldout.Draw("Overlay", meshMode);

            if (!value) return;

            EditorGUI.indentLevel++;

            var meshModeEnable = serializedObject.FindProperty("meshMode.enable");
            var meshModeAlpha = serializedObject.FindProperty("meshMode.alpha");
            var meshModeShader = serializedObject.FindProperty("meshMode.shader");

            EditorGUILayout.PropertyField(meshModeEnable, new GUIContent("Enable"));

            meshModeAlpha.floatValue = EditorGUILayout.Slider("Alpha", meshModeAlpha.floatValue, 0, 1);

            EditorGUILayout.PropertyField(meshModeShader, new GUIContent("Material"));

            if (meshModeShader.intValue == (int)MeshModeShader.Custom)
            {
                var value2 = GUIFoldout.Draw("Materials", meshMode.materials);

                if (value2)
                {
                    EditorGUI.indentLevel++;

                    var count = meshMode.materials.Length;
                    count = EditorGUILayout.IntSlider("Material Count", count, 0, 10);

                    if (count != meshMode.materials.Length) Array.Resize(ref meshMode.materials, count);

                    for (var id = 0; id < meshMode.materials.Length; id++)
                    {
                        var material = meshMode.materials[id];

                        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);

                        meshMode.materials[id] = material;
                    }

                    EditorGUI.indentLevel--;
                }
            }

            GUISortingLayer.Draw(serializedObject, meshMode.sortingLayer, "meshMode.");

            EditorGUI.indentLevel--;
        }
    }

    public class GUIBumpMapMode
    {
        public static void Draw(SerializedObject serializedObject, object obj)
        {
            // Serialized property
            var value = GUIFoldout.Draw("Mask Bump Map", obj);

            if (!value) return;

            EditorGUI.indentLevel++;

            var bumpType = serializedObject.FindProperty("bumpMapMode.type");
            var bumpTextureType = serializedObject.FindProperty("bumpMapMode.textureType");
            var bumpTexture = serializedObject.FindProperty("bumpMapMode.texture");
            var bumpSprite = serializedObject.FindProperty("bumpMapMode.sprite");

            var invertX = serializedObject.FindProperty("bumpMapMode.invertX");
            var invertY = serializedObject.FindProperty("bumpMapMode.invertY");

            var depth = serializedObject.FindProperty("bumpMapMode.depth");

            var spriteRenderer = serializedObject.FindProperty("bumpMapMode.spriteRenderer");
            var sr = (SpriteRenderer)spriteRenderer.objectReferenceValue;

            EditorGUILayout.PropertyField(bumpType, new GUIContent("Type"));
            EditorGUILayout.PropertyField(bumpTextureType, new GUIContent("Texture Type"));

            EditorGUILayout.PropertyField(invertX, new GUIContent("Invert X"));

            EditorGUILayout.PropertyField(invertY, new GUIContent("Invert Y"));

            EditorGUILayout.PropertyField(depth, new GUIContent("Depth"));

            switch (bumpTextureType.intValue)
            {
                case (int)NormalMapTextureType.Texture:

                    bumpTexture.objectReferenceValue = (Texture)EditorGUILayout.ObjectField("Texture",
                        bumpTexture.objectReferenceValue, typeof(Texture), true);

                    break;

                case (int)NormalMapTextureType.Sprite:

                    bumpSprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Sprite",
                        bumpSprite.objectReferenceValue, typeof(Sprite), true);

                    break;
            }

            EditorGUI.indentLevel--;
        }

        public static void DrawDay(DayNormalMapMode bumpMapMode)
        {
            var value = GUIFoldout.Draw("Mask Normal Map", bumpMapMode);

            if (!value) return;

            EditorGUI.indentLevel++;

            bumpMapMode.textureType =
                (NormalMapTextureType)EditorGUILayout.EnumPopup("Texture Type", bumpMapMode.textureType);

            switch (bumpMapMode.textureType)
            {
                case NormalMapTextureType.Texture:

                    bumpMapMode.texture =
                        (Texture)EditorGUILayout.ObjectField("Texture", bumpMapMode.texture, typeof(Texture), true);

                    break;

                case NormalMapTextureType.Sprite:

                    bumpMapMode.sprite =
                        (Sprite)EditorGUILayout.ObjectField("Sprite", bumpMapMode.sprite, typeof(Sprite), true);

                    break;
            }

            EditorGUI.indentLevel--;
        }
    }

    public class GUIGlowMode
    {
        public static void Draw(GlowMode glowMode)
        {
            var value = GUIFoldout.Draw("Glow Mode", glowMode);

            if (!value) return;

            EditorGUI.indentLevel++;

            glowMode.enable = EditorGUILayout.Toggle("Enable", glowMode.enable);

            glowMode.glowRadius = EditorGUILayout.Slider("Glow Size", glowMode.glowRadius, 0.1f, 10);

            EditorGUI.indentLevel--;
        }
    }
}