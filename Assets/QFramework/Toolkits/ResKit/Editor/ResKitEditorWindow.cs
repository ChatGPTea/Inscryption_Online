/****************************************************************************
 * Copyright (c) 2016 ~ 2023 liangxie
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class ResKitEditorWindow : EditorWindow
    {
        private static EncryptConfig mConfigInstance;

        private ResKitView mResKitView;

        public static bool EnableGenerateClass => EditorPrefs.GetBool(ResKitView.KEY_AUTOGENERATE_CLASS, false);


        private void OnEnable()
        {
            mResKitView = new ResKitView();
            mResKitView.EditorWindow = this;
            mResKitView.Init();
        }

        public void OnDisable()
        {
            //string savePath = Application.dataPath + "/QFrameworkData/Resources/EncryptConfig.Json";
            //using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            //    {
            //        sw.WriteLine(JsonUtility.ToJson(GetConfig()));
            //    }
            //}
            //AssetDatabase.Refresh();

            mResKitView.OnDispose();
            mResKitView = null;
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();


            mResKitView.OnGUI();


            GUILayout.EndVertical();
            GUILayout.Space(50);


            // RenderEndCommandExecuter.ExecuteCommand();
        }

        private static EncryptConfig GetConfig()
        {
            if (mConfigInstance == null)
            {
                var text = Resources.Load<TextAsset>("EncryptConfig");
                if (text)
                {
                    mConfigInstance = JsonUtility.FromJson<EncryptConfig>(text.text);

                    if (mConfigInstance == null) mConfigInstance = new EncryptConfig();
                }
                else
                {
                    mConfigInstance = new EncryptConfig();

                    var savePath = Application.dataPath + "QFrameworkData/Resources/EncryptConfig.Json";
                    using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                    {
                        using (var sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            sw.WriteLine(JsonUtility.ToJson(mConfigInstance));
                        }
                    }
                }
            }

            return mConfigInstance;
        }

        [MenuItem("QFramework/Toolkits/Res Kit %#r")]
        public static void OpenWindow()
        {
            var window = (ResKitEditorWindow)GetWindow(typeof(ResKitEditorWindow), true);
            Debug.Log(Screen.width + " screen width*****");
            window.position = new Rect(100, 100, 600, 400);
            window.Show();
        }
    }


    internal class EditorLifecycle
    {
        static EditorLifecycle()
        {
            EditorApplication.update += Update;
        }

        private static Queue<Action> mCommands { get; } = new();

        private static void Update()
        {
            ExecuteCommand();
        }

        public static void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0) mCommands.Dequeue().Invoke();
        }
    }
}