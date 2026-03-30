using UnityEditor;
using UnityEngine;

namespace FunkyCode
{
    public class Light2DWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private int tab;

        private readonly string[] toolbarString = { "Profile", "Project", "Debugging" };

        private void OnGUI()
        {
            tab = GUILayout.Toolbar(tab, toolbarString);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);

            EditorGUILayout.Space();

            switch (tab)
            {
                case 0:

                    ProfileEditor.Draw();

                    break;

                case 1:

                    ProjectSettingsEditor.Draw();

                    break;

                case 2:

                    DebuggingEditor.Debugging();

                    break;
            }

            GUILayout.EndScrollView();
        }

        public static Light2DWindow GetWindow()
        {
            var editorWindow = GetWindow<Light2DWindow>(false, "Lighting 2D", true);

            return editorWindow;
        }

        [MenuItem("Tools/Lighting 2D")]
        public static void ShowWindow()
        {
            UpdateWindow();
        }

        public static void UpdateWindow()
        {
            var editorWindow = GetWindow();

            editorWindow.minSize = new Vector2(350, 350);
            editorWindow.maxSize = new Vector2(4000, 4000);
        }
    }
}