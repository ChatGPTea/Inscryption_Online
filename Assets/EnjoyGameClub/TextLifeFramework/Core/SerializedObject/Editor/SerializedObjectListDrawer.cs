/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject.Editor
{
    [CustomPropertyDrawer(typeof(SerializedObjectList<>), true)]
    public class SerializedListDrawer : PropertyDrawer
    {
        private const string PROPERTY_NAME = "List";
        private const string TYPE_PROPERTY_NAME = "Type";
        private const int HEADER_HEIGHT = 30;
        private const int NAME_WIDTH = 200;
        private const int BUTTON_WIDTH = 80;
        private const int SPACE_WIDTH = 20;
        private Dictionary<string, ReorderableList> _reorderableListsMap;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var listProperty = property.FindPropertyRelative(PROPERTY_NAME);
            property.serializedObject.ApplyModifiedProperties();
            var list = GetReorderableList(listProperty, property, label);
            list?.DoList(position);
            listProperty.serializedObject.ApplyModifiedProperties();
            property.serializedObject.ApplyModifiedProperties();
        }


        private ReorderableList GetReorderableList(SerializedProperty listProperty, SerializedProperty property,
            GUIContent label)
        {
            var path = listProperty.propertyPath;
            _reorderableListsMap ??= new Dictionary<string, ReorderableList>();
            if (_reorderableListsMap.TryGetValue(path, out var list)) return list;
            list = CreateReorderableList(listProperty, property, label);
            _reorderableListsMap.Add(path, list);
            return list;
        }

        private ReorderableList CreateReorderableList(SerializedProperty listProperty, SerializedProperty property,
            GUIContent label)
        {
            var list = new ReorderableList(property.serializedObject, listProperty, false, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, label),
                drawElementCallback = (rect, index, isActive, isFocused) =>
                    DrawElement(rect, listProperty, property, index),
                elementHeightCallback = index => GetElementHeight(listProperty, index),
                onAddDropdownCallback = (rect, reorderableList) => OnAddDropDown(rect, listProperty, property),
                onRemoveCallback = reorderableList => OnRemoveElement(listProperty, reorderableList)
            };
            return list;
        }

        private void DrawElement(Rect rect, SerializedProperty listProperty, SerializedProperty property, int index)
        {
            property.serializedObject.Update();
            if (index > listProperty.arraySize - 1) return;

            // Rect.
            var elementRect = new Rect(rect.x + SPACE_WIDTH, rect.y, rect.width - 10,
                EditorGUIUtility.singleLineHeight);
            var objectRect = new Rect(NAME_WIDTH + SPACE_WIDTH, rect.y,
                rect.xMax - BUTTON_WIDTH - NAME_WIDTH - SPACE_WIDTH * 2,
                EditorGUIUtility.singleLineHeight);
            var buttonRect = new Rect(rect.xMax - BUTTON_WIDTH - SPACE_WIDTH, rect.y, BUTTON_WIDTH,
                EditorGUIUtility.singleLineHeight);

            var elementProperty = listProperty.GetArrayElementAtIndex(index);
            var typeName = elementProperty.objectReferenceValue != null
                ? elementProperty.objectReferenceValue.GetType().Name
                : "Null";
            property.serializedObject.ApplyModifiedProperties();

            // Draw element.
            EditorGUI.PropertyField(elementRect, elementProperty, new GUIContent(typeName), true);
            var callBackObject = EditorGUI.ObjectField(objectRect, elementProperty.objectReferenceValue,
                GetGenericType(property),
                false);
            if (!IsInScriptableObject(property) &&
                elementProperty.objectReferenceValue is IScriptableObjectSaveable saveableObj)
                if (GUI.Button(buttonRect, "Save"))
                {
                    saveableObj.Save(true);
                    GUIUtility.ExitGUI();
                }

            HandleObjectReferenceChange(callBackObject, elementProperty, property);
            property.serializedObject.ApplyModifiedProperties();
        }

        private void HandleObjectReferenceChange(Object newScriptableObject, SerializedProperty element,
            SerializedProperty property)
        {
            if (newScriptableObject == element.objectReferenceValue)
                return;
            // Changed
            if (newScriptableObject != null)
            {
                // Clone a ScriptableObject
                var name = newScriptableObject.name;
                newScriptableObject = (newScriptableObject as IScriptableObjectCloneable)?.Clone() as Object;
                newScriptableObject.name = name + "(Clone)";
            }

            element.objectReferenceValue = newScriptableObject;
            property.serializedObject.ApplyModifiedProperties();
        }

        private float GetElementHeight(SerializedProperty listProperty, int index)
        {
            var element = listProperty.GetArrayElementAtIndex(index);
            return EditorGUI.GetPropertyHeight(element) + 4;
        }

        private void OnAddDropDown(Rect rect, SerializedProperty listProperty, SerializedProperty property)
        {
            // elementProperty.objectReferenceValue = null;
            // property.serializedObject.ApplyModifiedProperties();
            var menu = new GenericMenu();
            var genericType = GetGenericType(property);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => genericType.IsAssignableFrom(type) && !type.IsAbstract);
            menu.AddItem(new GUIContent("None"), false, () =>
            {
                var index = listProperty.arraySize;
                listProperty.arraySize++;
                var elementProperty = listProperty.GetArrayElementAtIndex(index);
                elementProperty.objectReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });
            foreach (var type in types)
                menu.AddItem(new GUIContent(type.Name), false,
                    () =>
                    {
                        var index = listProperty.arraySize;
                        listProperty.arraySize++;
                        var elementProperty = listProperty.GetArrayElementAtIndex(index);
                        var newScriptableObject = ScriptableObject.CreateInstance(type);
                        newScriptableObject.name = type.Name;
                        if (IsInScriptableObject(property))
                        {
                            Undo.RegisterCreatedObjectUndo(newScriptableObject, "Create nested scriptable object");
                            Undo.undoRedoPerformed += UndoCallback;
                            AssetDatabase.AddObjectToAsset(newScriptableObject, property.serializedObject.targetObject);
                            AssetDatabase.SaveAssets();
                        }

                        elementProperty.objectReferenceValue = newScriptableObject;
                        property.serializedObject.ApplyModifiedProperties();
                    });

            menu.ShowAsContext();
        }

        private void UndoCallback()
        {
            AssetDatabase.SaveAssets();
            Undo.undoRedoPerformed -= UndoCallback;
        }

        private void OnRemoveElement(SerializedProperty listProperty, ReorderableList reorderableList)
        {
            if (reorderableList.index < 0)
                return;
            var elementProperty = listProperty.GetArrayElementAtIndex(reorderableList.index);
            if (elementProperty.objectReferenceValue == null)
            {
                listProperty.DeleteArrayElementAtIndex(reorderableList.index);
                listProperty.serializedObject.ApplyModifiedProperties();
                return;
            }

            Undo.DestroyObjectImmediate(elementProperty.objectReferenceValue);
            Undo.undoRedoPerformed += UndoCallback;
            AssetDatabase.SaveAssets();
            listProperty.DeleteArrayElementAtIndex(reorderableList.index);
            listProperty.serializedObject.ApplyModifiedProperties();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var listProp = property.FindPropertyRelative(PROPERTY_NAME);
            var list = GetReorderableList(listProp, property, label);
            if (list == null) return EditorGUIUtility.singleLineHeight;
            return list.GetHeight() + HEADER_HEIGHT;
        }

        private bool IsInScriptableObject(SerializedProperty property)
        {
            return AssetDatabase.Contains(property.serializedObject.targetObject);
        }

        private Type GetGenericType(SerializedProperty property)
        {
            // 获取父级对象
            object target = property.serializedObject.targetObject;
            if (target == null) return null;

            // 获取字段信息
            var field = target.GetType().GetField(property.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return null;

            // 获取字段类型
            var fieldType = field.FieldType;
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SerializedObjectList<>))
                return fieldType.GetGenericArguments()[0]; // 获取泛型参数T

            return null;
        }
    }
}