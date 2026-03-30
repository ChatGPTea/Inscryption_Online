using FunkyCode.LightingSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace FunkyCode
{
    public static class Lighting2DGizmoFiles
    {
        public static void Initialize()
        {
            var projectSettings = Lighting2D.ProjectSettings;

            if (projectSettings == null) return;

            if (projectSettings.gizmos.drawIcons == EditorIcons.Disabled) return;

            var icon_light = File.Exists("Assets/Gizmos/light_v2.png");

            if (!icon_light)
            {
                Debug.Log("false");

                try
                {
                    FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos", "Assets/Gizmos");
                }
                catch
                {
                }

                try
                {
                    FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/light_v2.png",
                        "Assets/Gizmos/light_v2.png");
                    FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/fow_v2.png",
                        "Assets/Gizmos/fow_v2.png");
                    FileUtil.CopyFileOrDirectory("Assets/FunkyCode/SmartLighting2D/Resources/Gizmos/circle_v2.png",
                        "Assets/Gizmos/circle_v2.png");
                }
                catch
                {
                }
            }
        }
    }

    [InitializeOnLoad]
    public class Lighting2DStartup
    {
        static Lighting2DStartup()
        {
            Lighting2DGizmoFiles.Initialize();
        }
    }
}