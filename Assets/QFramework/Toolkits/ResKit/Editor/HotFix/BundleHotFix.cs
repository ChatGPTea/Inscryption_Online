using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

//using NUnit.Framework;

namespace QFramework
{
    public class BundleHotFix : EditorWindow
    {
        private static readonly string m_BunleTargetPath =
            Application.streamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER;

        private static readonly string m_HotPath =
            Application.dataPath + "/../Hot/" + EditorUserBuildSettings.activeBuildTarget;

        private static readonly Dictionary<string, ABMD5> m_PackedMd5 = new();
        private string hotCount = "1";
        private OpenFileName m_OpenFileName;

        private string md5Path = "";

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            md5Path = EditorGUILayout.TextField("ABMD5路径： ", md5Path, GUILayout.Width(350), GUILayout.Height(20));
            if (GUILayout.Button("选择版本ABMD5文件", GUILayout.Width(150), GUILayout.Height(30)))
            {
                m_OpenFileName = new OpenFileName();
                m_OpenFileName.structSize = Marshal.SizeOf(m_OpenFileName);
                m_OpenFileName.filter = "ABMD5文件(*.bytes)\0*.bytes";
                m_OpenFileName.file = new string(new char[256]);
                m_OpenFileName.maxFile = m_OpenFileName.file.Length;
                m_OpenFileName.fileTitle = new string(new char[64]);
                m_OpenFileName.maxFileTitle = m_OpenFileName.fileTitle.Length;
                m_OpenFileName.initialDir = (Application.dataPath + "/../Version").Replace("/", "\\"); //默认路径
                m_OpenFileName.title = "选择MD5窗口";
                m_OpenFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
                if (LocalDialog.GetSaveFileName(m_OpenFileName)) md5Path = m_OpenFileName.file;
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            hotCount = EditorGUILayout.TextField("热更补丁版本：", hotCount, GUILayout.Width(350), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("开始打热更包", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (!string.IsNullOrEmpty(md5Path) && md5Path.EndsWith(".bytes"))
                {
                    BuildScript.BuildAssetBundles(EditorUserBuildSettings.activeBuildTarget);
                    ReadMD5Com(md5Path, hotCount);
                }

                DeleteMainfest();
                //加密AB包
                //EncryptAB();
            }
        }

        // [MenuItem("热更/打包热更包", false, 3)]
        private static void Init()
        {
            var window = (BundleHotFix)GetWindow(typeof(BundleHotFix), false, "热更包界面", true);
            window.Show();

            Debug.Log(m_BunleTargetPath);
            Debug.Log(m_HotPath);
        }

        private static void ReadMD5Com(string abmd5Path, string hotCount)
        {
            m_PackedMd5.Clear();

            using (var streamReader = new StreamReader(abmd5Path))
            {
                JsonUtility.FromJson<List<ABMD5>>(streamReader.ReadToEnd()).ForEach(_ =>
                {
                    m_PackedMd5.Add(_.ABName, _);
                });
            }

            var changeList = new List<string>();
            var directory = new DirectoryInfo(m_BunleTargetPath);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; i++)
                if (!files[i].Name.EndsWith(".meta") && !files[i].Name.EndsWith(".manifest"))
                {
                    var name = files[i].Name;
                    var md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                    Debug.Log("生成MD5" + files[i].FullName);
                    ABMD5 abmd5 = null;
                    if (!m_PackedMd5.ContainsKey(name))
                    {
                        changeList.Add(name);
                    }
                    else
                    {
                        if (m_PackedMd5.TryGetValue(name, out abmd5))
                            if (md5 != abmd5.MD5)
                                changeList.Add(name);
                    }
                }

            CopyAbAndGeneratJson(changeList, hotCount);
        }

        private static void CopyAbAndGeneratJson(List<string> changeList, string hotCount)
        {
            if (!Directory.Exists(m_HotPath)) Directory.CreateDirectory(m_HotPath);
            DeleteAllFile(m_HotPath);
            foreach (var str in changeList)
                if (!str.EndsWith(".manifest"))
                    File.Copy(m_BunleTargetPath + "/" + str, m_HotPath + "/" + str);

            //生成服务器Patch
            var directory = new DirectoryInfo(m_HotPath);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            var pathces = new Pathces();
            pathces.Version = 1;
            pathces.Files = new List<Patch>();
            for (var i = 0; i < files.Length; i++)
            {
                var patch = new Patch();
                patch.Md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
                patch.Name = files[i].Name;
                patch.Size = files[i].Length / 1024.0f;
                patch.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
                //服务器资源路径
                patch.Url = "http://annuzhiting2.oss-cn-hangzhou.aliyuncs.com/AssetBundle/" +
                            PlayerSettings.bundleVersion + "/" + hotCount + "/" + files[i].Name;
                pathces.Files.Add(patch);
            }

            BinarySerializeOpt.Xmlserialize(m_HotPath + "/Patch.xml", pathces);
        }


        //   [MenuItem("Tools/加密AB包")]
        public static void EncryptAB(string EncryptKey)
        {
            var directory = new DirectoryInfo(m_BunleTargetPath);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; i++)
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                    AES.AESFileEncrypt(files[i].FullName, EncryptKey);

            Debug.Log("加密完成！");
        }

        //  [MenuItem("Tools/解密AB包")]
        public static void DecrptyAB(string DecrptyKey)
        {
            var directory = new DirectoryInfo(m_BunleTargetPath);
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; i++)
                if (!files[i].Name.EndsWith("meta") && !files[i].Name.EndsWith(".manifest"))
                    AES.AESFileDecrypt(files[i].FullName, DecrptyKey);

            Debug.Log("解密完成！");
        }

        /// <summary>
        ///     删除指定文件目录下的所有文件
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static void DeleteAllFile(string fullPath)
        {
            if (Directory.Exists(fullPath))
            {
                var directory = new DirectoryInfo(fullPath);
                var files = directory.GetFiles("*", SearchOption.AllDirectories);
                for (var i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".meta")) continue;
                    File.Delete(files[i].FullName);
                }
            }
        }


        private static void DeleteMainfest()
        {
            var directoryInfo = new DirectoryInfo(m_BunleTargetPath);
            var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (var i = 0; i < files.Length; i++)
                if (files[i].Name.EndsWith(".manifest"))
                    File.Delete(files[i].FullName);
        }
    }
}