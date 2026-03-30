/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class LiveCodingKitSetting : ScriptableObject
    {
        public enum ReloadMethod
        {
            RestartGame = 0,
            ReloadCurrentScene = 1
        }

        private const string FileName = "Setting.asset";


        private static LiveCodingKitSetting mInstance;

        private static readonly Lazy<string> Dir =
            new(() => "Assets/QFrameworkData/LiveCodingKit/".CreateDirIfNotExists());

        public bool Open;


        public ReloadMethod WhenCompileFinish = ReloadMethod.ReloadCurrentScene;

        public static LiveCodingKitSetting Load()
        {
            if (mInstance) return mInstance;

            var filePath = Dir.Value + FileName;

            if (File.Exists(filePath)) return mInstance = AssetDatabase.LoadAssetAtPath<LiveCodingKitSetting>(filePath);

            return mInstance = CreateInstance<LiveCodingKitSetting>();
        }

        public void Save()
        {
            var filePath = Dir.Value + FileName;
            if (!File.Exists(filePath)) AssetDatabase.CreateAsset(this, Dir.Value + FileName);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif