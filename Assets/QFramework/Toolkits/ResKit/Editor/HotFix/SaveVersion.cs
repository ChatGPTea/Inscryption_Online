using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class SaveVersion : EditorWindow
    {
        private static readonly string m_VersionMd5Path =
            Application.dataPath + "/../Version/" + EditorUserBuildSettings.activeBuildTarget;

        private string VersionText;


        private void OnGUI()
        {
            if (GUILayout.Button("记录版本号", GUILayout.Width(200)))
            {
                //记录版本号

                var content = "Version|" + PlayerSettings.bundleVersion + ";PackageName|" +
                              PlayerSettings.applicationIdentifier + ";";
                var savePath = Application.dataPath + "/Resources/Version.txt";
                var oneLine = "";
                var all = "";
                using (var fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                           FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        all = sr.ReadToEnd();
                        oneLine = all.Split('\r')[0];
                    }
                }

                using (var fs = new FileStream(savePath, FileMode.OpenOrCreate))
                {
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        if (string.IsNullOrEmpty(all))
                            all = content;
                        else
                            all = all.Replace(oneLine, content);
                        sw.Write(all);
                    }
                }
            }

            if (GUILayout.Button("生成热更资源列表", GUILayout.Width(200)))
            {
                var path = (Application.streamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER)
                    .CreateDirIfNotExists();
                Debug.Log(path);
                var directoryInfo = new DirectoryInfo(path);
                var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                var abMD5List = new List<ABMD5>();
                for (var i = 0; i < files.Length; i++)
                    if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
                    {
                        var aBMD5 = new ABMD5(files[i].Name, files[i].Length / 1024.0f,
                            MD5Manager.Instance.BuildFileMd5(files[i].FullName));

                        abMD5List.Add(aBMD5);
                    }

                var ABMD5Path = Application.dataPath + "/Resources/ABMD5.bytes";

                File.WriteAllText(ABMD5Path, JsonUtility.ToJson(abMD5List));

                //将打版的版本拷贝到外部进行储存
                if (!Directory.Exists(m_VersionMd5Path)) Directory.CreateDirectory(m_VersionMd5Path);
                var targetPath = m_VersionMd5Path + "/ABMD5_" + PlayerSettings.bundleVersion + ".bytes";
                if (File.Exists(targetPath)) File.Delete(targetPath);
                File.Copy(ABMD5Path, targetPath);
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }


        // [MenuItem("热更/热更配置", false, 2)]
        private static void AddWindow()
        {
            //创建窗口
            var wr = new Rect(0, 0, 500, 200);
            var window = (SaveVersion)GetWindowWithRect(typeof(SaveVersion), wr, true, "热更版本号记录");
            window.Show();
        }
    }
}