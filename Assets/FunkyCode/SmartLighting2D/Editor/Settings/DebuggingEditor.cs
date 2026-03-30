using UnityEditor;
using UnityEngine;

namespace FunkyCode
{
    public static class DebuggingEditor
    {
        private static bool gameLightmapFoldout;
        private static bool sceneLightmapFoldout;

        private static bool lightFoldout;
        private static bool shaderPassFoldout;
        private static bool materialPassFoldout;

        private static bool camerasFoldout;

        private static readonly bool[] passFoldouts = new bool[9];

        private static readonly bool[] overlayFoldouts = new bool[20];

        public static void Debugging()
        {
            EditorGUI.indentLevel++;

            var gameCount = 0;

            foreach (var buffer in LightMainBuffer2D.List)
            {
                if (buffer.cameraSettings.cameraType == CameraSettings.CameraType.SceneView) continue;

                gameCount++;
            }

            gameLightmapFoldout =
                EditorGUILayout.Foldout(gameLightmapFoldout, "Game Lightmaps (" + gameCount + ")", true);

            if (gameLightmapFoldout)
            {
                EditorGUI.indentLevel++;

                var id = 0;

                foreach (var buffer in LightMainBuffer2D.List)
                {
                    var cameraSetting = buffer.cameraSettings;

                    if (cameraSetting.cameraType == CameraSettings.CameraType.SceneView) continue;

                    id++;

                    var cameraLightmap = buffer.cameraLightmap;

                    EditorGUILayout.ObjectField("Camera Target (" + cameraSetting.cameraType + ")",
                        cameraSetting.GetCamera(), typeof(Camera), true);
                    EditorGUILayout.Popup("Lightmap Preset (" + cameraLightmap.id + ")", cameraLightmap.presetId,
                        Lighting2D.Profile.lightmapPresets.GetLightmapLayers());
                    EditorGUILayout.EnumPopup("Rendering", cameraLightmap.rendering);
                    EditorGUILayout.EnumPopup("Render Texture Type", buffer.type);
                    EditorGUILayout.EnumPopup("Render Texture HDR", buffer.hdr);

                    var gui = EditorGUILayout.GetControlRect();

                    EditorGUI.ObjectField(gui, "Render Texture", buffer.renderTexture.renderTexture,
                        typeof(RenderTexture), true);

                    overlayFoldouts[id] = EditorGUILayout.Foldout(overlayFoldouts[id],
                        "Overlay (" + cameraLightmap.overlay + ")", true);

                    if (overlayFoldouts[id])
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.EnumPopup("Overlay Material Type", cameraLightmap.overlayMaterial);
                        EditorGUILayout.ObjectField("Overlay Material", buffer.GetMaterial(), typeof(Material), true);
                        EditorGUI.indentLevel--;
                    }

                    if (buffer.renderTexture != null)
                    {
                        var guiRect = EditorGUILayout.GetControlRect();

                        Texture texture = buffer.renderTexture.renderTexture;

                        var ratio = (float)texture.width / texture.height;

                        var width = 200 * ratio;
                        float height = 200;

                        var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                        EditorGUI.DrawPreviewTexture(drawRect, texture, null);


                        var GUIStyle = new GUIStyle();
                        GUIStyle.fontSize = 12;
                        GUIStyle.normal.textColor = Color.white;
                        GUIStyle.alignment = TextAnchor.LowerRight;

                        EditorGUI.LabelField(drawRect, texture.width + "x" + texture.height, GUIStyle);

                        GUILayout.Space(height);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
            }

            var sceneCount = 0;

            foreach (var buffer in LightMainBuffer2D.List)
            {
                if (buffer.cameraSettings.cameraType != CameraSettings.CameraType.SceneView) continue;

                sceneCount++;
            }

            sceneLightmapFoldout =
                EditorGUILayout.Foldout(sceneLightmapFoldout, "Scene Lightmaps (" + sceneCount + ")", true);

            if (sceneLightmapFoldout)
            {
                EditorGUI.indentLevel++;

                var id = 0;

                foreach (var buffer in LightMainBuffer2D.List)
                {
                    var cameraSetting = buffer.cameraSettings;

                    if (cameraSetting.cameraType != CameraSettings.CameraType.SceneView) continue;

                    id++;

                    var cameraLightmap = buffer.cameraLightmap;

                    EditorGUILayout.ObjectField("Camera Target (" + cameraSetting.cameraType + ")",
                        cameraSetting.GetCamera(), typeof(Camera), true);
                    EditorGUILayout.Popup("Lightmap Preset (" + cameraLightmap.id + ")", cameraLightmap.presetId,
                        Lighting2D.Profile.lightmapPresets.GetLightmapLayers());
                    EditorGUILayout.EnumPopup("Rendering", cameraLightmap.rendering);
                    EditorGUILayout.EnumPopup("Render Texture Type", buffer.type);
                    EditorGUILayout.EnumPopup("Render Texture HDR", buffer.hdr);

                    var gui = EditorGUILayout.GetControlRect();

                    EditorGUI.ObjectField(gui, "Render Texture", buffer.renderTexture.renderTexture,
                        typeof(RenderTexture), true);

                    overlayFoldouts[id] = EditorGUILayout.Foldout(overlayFoldouts[id],
                        "Overlay (" + cameraLightmap.overlay + ")", true);

                    if (overlayFoldouts[id])
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.EnumPopup("Overlay Material Type", cameraLightmap.overlayMaterial);
                        EditorGUILayout.ObjectField("Overlay Material", buffer.GetMaterial(), typeof(Material), true);
                        EditorGUI.indentLevel--;
                    }

                    if (buffer.renderTexture != null)
                    {
                        var guiRect = EditorGUILayout.GetControlRect();

                        Texture texture = buffer.renderTexture.renderTexture;

                        var ratio = (float)texture.width / texture.height;

                        var width = 200 * ratio;
                        float height = 200;

                        var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                        EditorGUI.DrawPreviewTexture(drawRect, texture, null);


                        var GUIStyle = new GUIStyle();
                        GUIStyle.fontSize = 12;
                        GUIStyle.normal.textColor = Color.white;
                        GUIStyle.alignment = TextAnchor.LowerRight;

                        EditorGUI.LabelField(drawRect, texture.width + "x" + texture.height, GUIStyle);

                        GUILayout.Space(height);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
            }

            var taken = 0;

            foreach (var buffer in LightBuffer2D.List)
                if (!buffer.Free)
                    taken += 1;

            lightFoldout = EditorGUILayout.Foldout(lightFoldout,
                "Light Shadowmaps (" + taken + "/" + LightBuffer2D.List.Count + ")", true);

            if (lightFoldout)
            {
                EditorGUI.indentLevel++;

                foreach (var buffer in LightBuffer2D.List)
                {
                    var guiRect = EditorGUILayout.GetControlRect();

                    if (!buffer.Free)
                        EditorGUI.ObjectField(new Rect(guiRect.x, guiRect.y, guiRect.width - 100, guiRect.height),
                            "Source", buffer.Light, typeof(Light2D), true);
                    else
                        EditorGUI.LabelField(new Rect(guiRect.x, guiRect.y, guiRect.width - 100, guiRect.height),
                            "Source: Free");

                    if (buffer.renderTexture != null)
                    {
                        // EditorGUILayout.ObjectField("Render Texture", buffer.renderTexture.renderTexture, typeof(Texture), true);

                        Texture texture = buffer.renderTexture.renderTexture;

                        EditorGUI.ObjectField(new Rect(guiRect.x, guiRect.y + 20, guiRect.width - 100, guiRect.height),
                            "Texture", texture, typeof(RenderTexture), true);
                        EditorGUI.LabelField(new Rect(guiRect.x, guiRect.y + 40, guiRect.width, guiRect.height),
                            buffer.name);

                        float width = 100;
                        float height = 100;

                        var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                        EditorGUI.DrawPreviewTexture(drawRect, texture, null);

                        var GUIStyle = new GUIStyle();
                        GUIStyle.fontSize = 12;
                        GUIStyle.normal.textColor = Color.white;
                        GUIStyle.alignment = TextAnchor.LowerRight;

                        EditorGUI.LabelField(drawRect, texture.width.ToString(), GUIStyle);


                        GUILayout.Space(height);
                    }

                    if (buffer.translucencyTexture != null)
                    {
                        guiRect = EditorGUILayout.GetControlRect();

                        Texture texture = buffer.translucencyTexture.renderTexture;

                        EditorGUI.LabelField(new Rect(guiRect.x, guiRect.y + 20, guiRect.width, guiRect.height),
                            "Translucency Texture");

                        var ratio = (float)texture.width / texture.height;

                        var width = 100 * ratio;
                        float height = 100;

                        var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                        EditorGUI.DrawPreviewTexture(drawRect, texture, null);

                        var GUIStyle = new GUIStyle();
                        GUIStyle.fontSize = 15;
                        GUIStyle.normal.textColor = Color.white;
                        GUIStyle.alignment = TextAnchor.LowerRight;

                        EditorGUI.LabelField(drawRect, texture.width.ToString(), GUIStyle);

                        GUILayout.Space(height);
                    }

                    /*

                    if (buffer.collisionTextureBlur != null)
                    {
                        EditorGUILayout.ObjectField("Collision Texture (Post)", buffer.collisionTextureBlur.renderTexture, typeof(Texture), true);
                    }

                    */

                    if (buffer.freeFormTexture != null)
                        EditorGUILayout.ObjectField("Free Form Texture", buffer.freeFormTexture.renderTexture,
                            typeof(Texture), true);

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
            }

            var passCount = 0;

            for (var i = 1; i <= 8; i++)
                if (NameType(i) > 0)
                    passCount++;

            shaderPassFoldout = EditorGUILayout.Foldout(shaderPassFoldout, "Shader Passes (" + passCount + ")", true);

            if (shaderPassFoldout)
            {
                EditorGUI.indentLevel++;

                for (var i = 1; i <= 8; i++)
                    if (NameType(i) > 0)
                        DrawPass(i);

                EditorGUI.indentLevel--;
            }

            materialPassFoldout = EditorGUILayout.Foldout(materialPassFoldout,
                "Material Passes (" + MaterialSystem.Count + ")", true);

            if (materialPassFoldout)
            {
                EditorGUI.indentLevel++;

                for (var i = 0; i < MaterialSystem.Count; i++)
                {
                    var pass = MaterialSystem.materialPasses[i];

                    EditorGUILayout.LabelField("[PassId: " + pass.passId + "] -> " + pass.material.name + " " +
                                               pass.material.shader.name);
                }

                EditorGUI.indentLevel--;
            }

            camerasFoldout =
                EditorGUILayout.Foldout(camerasFoldout, "Cameras (" + CameraTransform.List.Count + ")", true);

            if (camerasFoldout)
            {
                EditorGUI.indentLevel++;

                foreach (var camera in CameraTransform.List) EditorGUILayout.LabelField(camera.Camera.name);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            EditorGUI.indentLevel--;


            /*

            EditorGUILayout.Foldout(true, "Internal");

            EditorGUI.indentLevel++;

            EditorGUILayout.ObjectField("Mask Material", Lighting2D.materials.mask.GetMask(), typeof(Material), true);

            EditorGUI.indentLevel--;
            */
        }

        public static void DrawPass(int id)
        {
            var typeId = NameType(id);

            var nameType = "";

            switch (typeId)
            {
                case 0:
                    nameType = "NULL";
                    break;

                case 1:
                    nameType = "Game";
                    break;

                case 2:
                    nameType = "Scene";
                    break;

                case 3:
                    nameType = "Game & Scene";
                    break;
            }

            passFoldouts[id] = EditorGUILayout.Foldout(passFoldouts[id], "Pass " + id + " (" + nameType + ")", true);

            if (!passFoldouts[id]) return;

            EditorGUI.indentLevel++;

            if (typeId == 1 || typeId == 3)
            {
                var guiRect = EditorGUILayout.GetControlRect();

                var rect = Shader.GetGlobalVector("_GameRect" + id);

                var rot = Shader.GetGlobalFloat("_GameRotation" + id);

                EditorGUI.LabelField(new Rect(guiRect.x, guiRect.y, guiRect.width, guiRect.height),
                    "Game Vector " + Mathf.Ceil(rect.x) + " " + Mathf.Ceil(rect.y) + " " + Mathf.Ceil(rect.z) + " " +
                    Mathf.Ceil(rect.w) + " " + rot);

                var texture = Shader.GetGlobalTexture("_GameTexture" + id);

                if (texture != null)
                {
                    var ratio = (float)texture.width / texture.height;

                    var width = 100 * ratio;
                    float height = 100;

                    var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                    EditorGUI.DrawPreviewTexture(drawRect, texture, null);

                    var GUIStyle = new GUIStyle();
                    GUIStyle.fontSize = 12;
                    GUIStyle.normal.textColor = Color.white;
                    GUIStyle.alignment = TextAnchor.LowerRight;

                    EditorGUI.LabelField(drawRect, texture.width + "x" + texture.height, GUIStyle);

                    GUILayout.Space(height);
                }
            }

            if (typeId == 2 || typeId == 3)
            {
                var guiRect = EditorGUILayout.GetControlRect();

                var rect = Shader.GetGlobalVector("_SceneRect" + id);

                var rot = Shader.GetGlobalFloat("_SceneRotation" + id);

                EditorGUI.LabelField(new Rect(guiRect.x, guiRect.y, guiRect.width, guiRect.height),
                    "Scene Vector " + Mathf.Ceil(rect.x) + " " + Mathf.Ceil(rect.y) + " " + Mathf.Ceil(rect.z) + " " +
                    Mathf.Ceil(rect.w) + " " + rot);

                var texture = Shader.GetGlobalTexture("_SceneTexture" + id);

                if (texture != null)
                {
                    var ratio = (float)texture.width / texture.height;

                    var width = 100 * ratio;
                    float height = 100;

                    var drawRect = new Rect(guiRect.x + guiRect.width - width, guiRect.y, width, height);

                    EditorGUI.DrawPreviewTexture(drawRect, texture, null);

                    var GUIStyle = new GUIStyle();
                    GUIStyle.fontSize = 12;
                    GUIStyle.normal.textColor = Color.white;
                    GUIStyle.alignment = TextAnchor.LowerRight;

                    EditorGUI.LabelField(drawRect, texture.width + "x" + texture.height, GUIStyle);

                    GUILayout.Space(height);
                }

                EditorGUI.indentLevel--;
            }
        }

        public static int NameType(int id)
        {
            var gameExist = Shader.GetGlobalVector("_GameRect" + id).z > 0;

            gameExist = gameExist || Shader.GetGlobalTexture("_GameTexture" + id) != null;

            var sceneExist = Shader.GetGlobalVector("_SceneRect" + id).z > 0;

            sceneExist = sceneExist || Shader.GetGlobalTexture("_SceneTexture" + id) != null;

            if (gameExist && !sceneExist) return 1;

            if (!gameExist && sceneExist) return 2;

            if (gameExist && sceneExist) return 3;

            return 0;
        }
    }
}