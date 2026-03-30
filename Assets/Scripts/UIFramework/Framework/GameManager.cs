using System;
using System.Collections;
using System.Runtime.InteropServices;
using QFramework;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Inscryption
{
    public class GameManager : InscryptionController
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // 正确导入 Windows API
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        [DllImport("kernel32.dll", SetLastError = true)]
        
        private static extern bool SetConsoleCP(uint wCodePageID);
#endif
        private readonly float SoundVolume = 0.3f;

        public static GameManager Instance;

        public GameObject mLight1;
        public GameObject mLight2;

        private void Start()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            // 设置控制台输出为 UTF-8 (代码页 65001)
            SetConsoleOutputCP(65001);
        
            // 同时设置 C# 控制台编码
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
#endif
#if UNITY_ANDROID
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
            Instance = this;
            StartCoroutine(GameInit());
        }

        private IEnumerator GameInit()
        {
            Debug.Log("===== GameInit 开始 =====");
    
            // 1. 异步加载配置
            yield return ConfigManager.Instance.Initial();

            // 2. 到这里 ConfigInfo 一定不为 null（除非加载失败）
            if (ConfigManager.Instance.ConfigInfo.role == 0)
            {
                Debug.Log("我是服务器，启动 Host...");
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetConnectionData("0.0.0.0", (ushort)ConfigManager.Instance.ConfigInfo.port);
                NetworkManager.Singleton.StartServer();
                Debug.Log("StartServer 调用完成");
            }
            else
            {
                Debug.Log("我是客户端，初始化 UI...");
                // 客户端初始化 UI 等
                TurnOnAndDownLight(false);

                var canvas = GameObject.Find("Canvas").transform;
                WndManager.Instance.Initial(canvas);
                WndManager.Instance.OpenWnd<StartWnd>();

                AudioKit.Settings.SoundVolume.Value = SoundVolume;
                Debug.Log("客户端 UI 初始化完成");
            }
    
            Debug.Log("===== GameInit 结束 =====");
        }

        public void TurnOnAndDownLight(bool on)
        {
            if (mLight1 != null) mLight1.SetActive(on);
            if (mLight2 != null) mLight2.SetActive(on);
        }
    }
}