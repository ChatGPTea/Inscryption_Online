/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.Editor
{
    [CustomPropertyDrawer(typeof(AnimationProcess), true)]
    public class ProcessDrawer : PropertyDrawer
    {
        private const float WIDTH_OFFSET = 10;
        private const float X_OFFSET = 10;
        private const float PROPERTY_X_OFFSET = 20;
        private const int HEADER_FONT_SIZE = 14;
        private const int HEADER_HEIGHT = 30;
        private Color _borderColor;

        private readonly Dictionary<Type, List<FieldInfo>> _dictionary = new();


        private GUIStyle _headerStyle;
        private bool _isFolder = true;
        private readonly Dictionary<Object, UnityEditor.SerializedObject> _reorderableListMap = new();

        public ProcessDrawer()
        {
            Init();
        }

        private void Init()
        {
            _headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = Mathf.RoundToInt(HEADER_FONT_SIZE),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(0, 0, 3, 3),
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin
                        ? new Color(0.8f, 0.8f, 0.8f)
                        : Color.black
                }
            };
            _borderColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var currentObject = property.objectReferenceValue;
            if (currentObject == null) return;
            var animationProcessSerializedObject = GetSerializedObject(currentObject);
            animationProcessSerializedObject.Update();
            // 绘制下拉菜单
            _isFolder = EditorGUI.Foldout(position, _isFolder, label);
            if (_isFolder)
            {
                // 绘制边框
                EditorGUI.DrawRect(
                    new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width - WIDTH_OFFSET,
                        GetPropertyHeight(property, label) - EditorGUIUtility.singleLineHeight), _borderColor);
                position.y += EditorGUIUtility.singleLineHeight + 1;

                // 绘制属性
                var nameStr = EditorGUI.TextField(new Rect(position.x + X_OFFSET, position.y,
                        position.width - WIDTH_OFFSET - PROPERTY_X_OFFSET,
                        HEADER_HEIGHT), animationProcessSerializedObject.targetObject.name,
                    _headerStyle);
                animationProcessSerializedObject.targetObject.name = nameStr;
                position.y += HEADER_HEIGHT;
                var fields = GetBaseClassFields(currentObject.GetType(), typeof(AnimationProcess));
                foreach (var field in fields)
                {
                    var fieldProp = animationProcessSerializedObject.FindProperty(field.Name);
                    if (fieldProp != null)
                    {
                        var fieldHeight = EditorGUI.GetPropertyHeight(fieldProp, true);
                        EditorGUI.PropertyField(
                            new Rect(position.x + X_OFFSET, position.y,
                                position.width - WIDTH_OFFSET - PROPERTY_X_OFFSET,
                                fieldHeight),
                            fieldProp,
                            true);
                        position.y += fieldHeight + 2;
                        animationProcessSerializedObject.ApplyModifiedProperties();
                    }
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        private UnityEditor.SerializedObject GetSerializedObject(Object obj)
        {
            if (!_reorderableListMap.TryGetValue(obj, out var scriptableObject))
            {
                scriptableObject = new UnityEditor.SerializedObject(obj);
                _reorderableListMap.Add(obj, scriptableObject);
            }

            return scriptableObject;
        }

        private List<FieldInfo> GetBaseClassFields(Type type, Type stopType)
        {
            if (_dictionary.TryGetValue(type, out var fieldInfos)) return fieldInfos;

            var baseType = type;
            Stack<FieldInfo[]> fields = new();
            while (type != null && type != stopType.BaseType)
            {
                fields.Push(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
                type = type.BaseType;
            }

            fieldInfos = new List<FieldInfo>();
            while (fields.Count > 0) fieldInfos.AddRange(fields.Pop());

            _dictionary.Add(baseType, fieldInfos);
            return fieldInfos;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_isFolder) return EditorGUIUtility.singleLineHeight;

            var height = EditorGUIUtility.singleLineHeight + 4; // 标题的高度
            var currentObject = property.objectReferenceValue;
            if (currentObject == null) return height + 30; // 额外留出 Null 提示的高度

            var animationProcessScriptableObject = new UnityEditor.SerializedObject(currentObject);

            var objectType = currentObject.GetType();
            var fields = GetBaseClassFields(objectType, typeof(AnimationProcess));

            height += HEADER_HEIGHT;
            foreach (var field in fields)
            {
                var fieldProp = animationProcessScriptableObject.FindProperty(field.Name);
                if (fieldProp != null) height += EditorGUI.GetPropertyHeight(fieldProp, true) + 2;
            }

            return height;
        }
    }
}