/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using UnityEditor;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Editor
{
    [CustomEditor(typeof(TextLife))]
    public class TextLifeCustomEditor : UnityEditor.Editor
    {
        private const float TITLE_FONT_SIZE = 20f;
        private const float SPACING = 10f;
        private const int MIN_TEXTAREA_HEIGHT = 40;
        private const int MAX_TEXTAREA_HEIGHT = 300;
        private SerializedProperty _debugProperty;
        private SerializedProperty _fixedTimeProperty;
        private SerializedProperty _globalProcessesProperty;
        private SerializedProperty _matchConfigProperty;
        private SerializedProperty _matchProcessesProperty;

        private TextLife _target;
        private SerializedProperty _textProperty;
        private GUIStyle _titleStyle;
        private Texture2D icon;

        private void OnEnable()
        {
            _target = target as TextLife;
            _textProperty = serializedObject.FindProperty("Text");
            _matchProcessesProperty = serializedObject.FindProperty("MatchProcesses");
            _globalProcessesProperty = serializedObject.FindProperty("GlobalProcesses");
            _debugProperty = serializedObject.FindProperty("Debug");
            _matchConfigProperty = serializedObject.FindProperty("MatchConfig");
            _fixedTimeProperty = serializedObject.FindProperty("DisableTimeScale");
            InitStyle();
        }

        private void InitStyle()
        {
            _titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = Mathf.RoundToInt(TITLE_FONT_SIZE),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin
                        ? new Color(0.8f, 0.8f, 0.8f)
                        : Color.black
                }
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspectorHeader();
            DrawActionButtons();
            DrawList("Match Processes", _matchProcessesProperty);
            DrawList("Global Processes", _globalProcessesProperty);
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDefaultInspectorHeader()
        {
            EditorGUILayout.Space(SPACING * 0.5f);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Debug Text", _titleStyle, GUILayout.Height(40));
            DrawUnderline(Color.gray);
            var textAreaStyle = new GUIStyle(EditorStyles.textArea);
            var textHeight = textAreaStyle.CalcHeight(new GUIContent(_textProperty.stringValue),
                EditorGUIUtility.currentViewWidth - 20);
            textHeight = Mathf.Clamp(textHeight, MIN_TEXTAREA_HEIGHT, MAX_TEXTAREA_HEIGHT);
            _textProperty.stringValue =
                EditorGUILayout.TextArea(_textProperty.stringValue, textAreaStyle, GUILayout.Height(textHeight));
            EditorGUILayout.PropertyField(_debugProperty);
            EditorGUILayout.PropertyField(_fixedTimeProperty);
            EditorGUILayout.PropertyField(_matchConfigProperty);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(SPACING);
        }

        private void DrawList(string listName, SerializedProperty listProperty)
        {
            EditorGUILayout.LabelField(listName, _titleStyle, GUILayout.Height(40));
            DrawUnderline(Color.gray);
            EditorGUILayout.Space(SPACING * 0.5f);
            EditorGUILayout.PropertyField(listProperty);
        }

        private void DrawActionButtons()
        {
            EditorGUILayout.Space(SPACING);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Replay Animation", GUILayout.Width(150)))
                    if (_target != null)
                        _target.ResetAnimation();

                if (GUILayout.Button("Refresh", GUILayout.Width(80))) serializedObject.UpdateIfRequiredOrScript();
            }
        }

        private void DrawUnderline(Color color, float thickness = 2f)
        {
            var lastRect = GUILayoutUtility.GetLastRect(); // 获取上一行的控件区域
            var yPosition = lastRect.yMax + 1; // 线条位置（稍微往下偏移）
            EditorGUI.DrawRect(new Rect(lastRect.x, yPosition, lastRect.width, thickness), color);
            EditorGUILayout.Space(SPACING * 0.5f);
        }
    }
}