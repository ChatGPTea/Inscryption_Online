/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(10)]
    [DisplayNameCN("LiveCodingKit 设置")]
    [DisplayNameEN("LiveCodingKit Setting")]
    internal class LiveCodingKitSettingEditor : IPackageKitView
    {
        private readonly Lazy<GUIStyle> mLabel12 = new(() =>
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
        });

        private Lazy<GUIStyle> mLabelBold12 = new(() =>
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };
        });

        public EditorWindow EditorWindow { get; set; }

        public void Init()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();

                GUILayout.Label(LocaleText.Switch, mLabel12.Value, GUILayout.Width(40));

                LiveCodingKit.Setting.Open = GUILayout.Toggle(LiveCodingKit.Setting.Open, "");

                if (EditorGUI.EndChangeCheck()) Save();

                GUILayout.EndHorizontal();

                if (LiveCodingKit.Setting.Open)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(LocaleText.WhenCompileFinish, mLabel12.Value);
                    EditorGUI.BeginChangeCheck();

                    if (LocaleKitEditor.IsCN.Value)
                        LiveCodingKit.Setting.WhenCompileFinish =
                            (LiveCodingKitSetting.ReloadMethod)EditorGUILayout.Popup((int)LiveCodingKit.Setting
                                .WhenCompileFinish, LocaleText.ReloadMethodNames);
                    else
                        LiveCodingKit.Setting.WhenCompileFinish =
                            (LiveCodingKitSetting.ReloadMethod)EditorGUILayout.EnumPopup(LiveCodingKit.Setting
                                .WhenCompileFinish);

                    if (EditorGUI.EndChangeCheck()) Save();
                }
            }
            GUILayout.EndVertical();
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
        }

        public void OnShow()
        {
        }

        public void OnHide()
        {
        }

        private void Save()
        {
            LiveCodingKitSetting.Load().Save();
        }


        private class LocaleText
        {
            public static readonly string[] ReloadMethodNames =
            {
                RestartGame,
                ReloadCurrentScene
            };

            public static bool IsCN => LocaleKitEditor.IsCN.Value;

            public static string Switch =>
                IsCN ? " 开启：" : "Open:";


            public static string Apply => IsCN ? "保存" : "Apply";
            public static string WhenCompileFinish => IsCN ? "当编译完成时" : "When Compile Finish";
            private static string ReloadCurrentScene => IsCN ? "重新加载当前场景" : "";
            private static string RestartGame => IsCN ? "重启游戏" : "";
        }
    }
}
#endif