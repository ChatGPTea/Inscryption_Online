using System;
using System.IO;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public class AbstractBuildConfig<T> where T : AbstractBuildConfig<T>, new()
    {
        private static T mDefault;
        public static string FileName => typeof(T).Name + "_config";

        public static T Default => mDefault = mDefault ?? Load();

        public static T Load()
        {
            try
            {
                var textAsset = Resources.Load<TextAsset>($"BuildConfig/{FileName}");
                return JsonUtility.FromJson<T>(textAsset.text);
            }
            catch
            {
                return new T();
            }
        }


#if UNITY_EDITOR
        public const string FolderPath = "Assets/Art/Config/Resources/BuildConfig/";
        public static string FilePath => FolderPath.CreateDirIfNotExists() + FileName + ".json";

        public void Save()
        {
            var jsonContent = JsonUtility.ToJson(this, true);
            File.WriteAllText(FilePath, jsonContent);
        }
#endif
    }
}