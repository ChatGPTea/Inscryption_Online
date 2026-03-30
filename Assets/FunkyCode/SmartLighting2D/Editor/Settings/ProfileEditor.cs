using System;
using FunkyCode.LightingSettings;
using FunkyCode.LightSettings;
using UnityEditor;
using UnityEngine;

namespace FunkyCode
{
    public class ProfileEditor
    {
        public static void DrawProfile(Profile profile)
        {
            EditorGUI.BeginChangeCheck();

            // Common Settings

            CommonSettings(profile.lightmapPresets.list[0]);

            EditorGUILayout.Space();

            // Quality Settings

            QualitySettings.Draw(profile);

            EditorGUILayout.Space();

            // Layers

            Layers.Draw(profile);

            EditorGUILayout.Space();

            // Day Lighting

            DayLighting.Draw(profile);

            EditorGUILayout.Space();

            // Lightmap Presets

            LightmapPresets.Draw(profile.lightmapPresets);

            EditorGUILayout.Space();

            // Light Presets

            LightPresets.Draw(profile.lightPresets);

            EditorGUILayout.Space();

            // Event Presets

            EventPresets.Draw(profile.eventPresets);

            EditorGUILayout.Space();

            EditorGUI.EndChangeCheck();

            if (GUI.changed)
                if (!EditorApplication.isPlaying)
                {
                    if (Lighting2D.Profile == profile)
                    {
                        Light2D.ForceUpdateAll();

                        LightingManager2D.ForceUpdate();
                        /*

                        foreach(OnRenderMode onRender in OnRenderMode.List) {
                            LightmapPreset lightmapPreset = onRender.mainBuffer.GetLightmapPreset();
                            lightmapPreset.sortingLayer.ApplyToMeshRenderer(onRender.meshRenderer);
                        }*/
                    }

                    EditorUtility.SetDirty(profile);
                }
        }

        public static void Draw()
        {
            var profile = Lighting2D.Profile;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Current Profile", profile, typeof(Profile), true);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            if (profile == null)
            {
                EditorGUILayout.HelpBox("Lighting2D Settings Profile Not Found!", MessageType.Error);

                return;
            }

            DrawProfile(profile);
        }

        public static void DrawEventLayers(EventPresetLayers presetLayers)
        {
            var layerSetting = presetLayers.Get();

            var layerCount = layerSetting.Length;

            layerCount = EditorGUILayout.IntSlider("Layer Count", layerCount, 1, 4);

            EditorGUILayout.Space();

            if (layerCount != layerSetting.Length)
            {
                var oldCount = layerSetting.Length;

                Array.Resize(ref layerSetting, layerCount);

                for (var i = oldCount; i < layerCount; i++)
                    if (layerSetting[i] == null)
                    {
                        layerSetting[i] = new LayerEventSetting();
                        layerSetting[i].layerID = i;
                    }

                presetLayers.SetArray(layerSetting);
            }

            for (var i = 0; i < layerSetting.Length; i++)
            {
                var layer = layerSetting[i];

                layer.layerID = EditorGUILayout.Popup(" ", layer.layerID,
                    Lighting2D.Profile.layers.colliderLayers.GetNames());
            }
        }

        public static void DrawLightLayers(LightPresetLayers presetLayers)
        {
            var layerSetting = presetLayers.Get();

            var layerCount = layerSetting.Length;

            layerCount = EditorGUILayout.IntSlider("Layer Count", layerCount, 1, 8);

            EditorGUILayout.Space();

            if (layerCount != layerSetting.Length)
            {
                var oldCount = layerSetting.Length;

                Array.Resize(ref layerSetting, layerCount);

                for (var i = oldCount; i < layerCount; i++)
                    if (layerSetting[i] == null)
                    {
                        layerSetting[i] = new LayerSetting();
                        layerSetting[i].layerID = i;
                    }

                presetLayers.SetArray(layerSetting);
            }

            for (var i = 0; i < layerSetting.Length; i++)
            {
                var layer = layerSetting[i];

                var foldout = GUIFoldout.Draw("Layer " + (i + 1), layer);

                if (foldout)
                {
                    EditorGUI.indentLevel++;

                    layer.layerID = EditorGUILayout.Popup("Layer (Collider)", layer.layerID,
                        Lighting2D.Profile.layers.colliderLayers.GetNames());

                    layer.type = (LightLayerType)EditorGUILayout.EnumPopup("Type", layer.type);

                    var shadowEnabled = layer.type != LightLayerType.MaskOnly;
                    var maskEnabled = layer.type != LightLayerType.ShadowOnly;

                    EditorGUILayout.Space();

                    layer.sorting = (LightLayerSorting)EditorGUILayout.EnumPopup("Sorting", layer.sorting);

                    EditorGUI.BeginDisabledGroup(layer.sorting == LightLayerSorting.None);

                    layer.sortingIgnore =
                        (LightLayerSortingIgnore)EditorGUILayout.EnumPopup("Sorting Ignore", layer.sortingIgnore);

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    EditorGUI.BeginDisabledGroup(!shadowEnabled);

                    layer.shadowEffect =
                        (LightLayerShadowEffect)EditorGUILayout.EnumPopup("Shadow Effect", layer.shadowEffect);

                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(!shadowEnabled ||
                                                 layer.shadowEffect != LightLayerShadowEffect.PerpendicularProjection);

                    layer.shadowEffectLayer = EditorGUILayout.Popup("Effect Layer (Collider)", layer.shadowEffectLayer,
                        Lighting2D.Profile.layers.colliderLayers.GetNames());

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    EditorGUI.BeginDisabledGroup(!maskEnabled);

                    layer.maskLit = (LightLayerMaskLit)EditorGUILayout.EnumPopup("Mask Lit", layer.maskLit);

                    EditorGUI.EndDisabledGroup();

                    var maskEffectLit = layer.maskLit == LightLayerMaskLit.AboveLit;

                    EditorGUI.BeginDisabledGroup(!maskEnabled || !maskEffectLit);

                    layer.maskLitDistance = EditorGUILayout.FloatField("Mask Lit Distance", layer.maskLitDistance);

                    if (layer.maskLitDistance < 0) layer.maskLitDistance = 0;

                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.Space();

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }
        }

        private static void CommonSettings(LightmapPreset lightmapPreset)
        {
            if (lightmapPreset.type != LightmapPreset.Type.Depth8)
            {
                lightmapPreset.darknessColor =
                    EditorGUILayout.ColorField("Darkness Color", lightmapPreset.darknessColor);
                lightmapPreset.darknessColor.a =
                    EditorGUILayout.Slider("Darkness Alpha", lightmapPreset.darknessColor.a, 0, 1);
                lightmapPreset.resolution =
                    EditorGUILayout.Slider("Resolution", lightmapPreset.resolution, 0.25f, 1.0f);
            }
        }

        public class LightPresets
        {
            public static void Draw(LightPresetList lightPresetList)
            {
                var foldout = GUIFoldoutHeader.Begin("Light Presets (" + lightPresetList.list.Length + ")",
                    lightPresetList);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                var presetCount = EditorGUILayout.IntSlider("Count", lightPresetList.list.Length, 1, 8);

                if (presetCount != lightPresetList.list.Length)
                {
                    var oldCount = lightPresetList.list.Length;

                    Array.Resize(ref lightPresetList.list, presetCount);

                    for (var i = oldCount; i < presetCount; i++) lightPresetList.list[i] = new LightPreset(i);
                }

                for (var i = 0; i < lightPresetList.list.Length; i++)
                {
                    var lightPreset = lightPresetList.list[i];

                    var fold = GUIFoldout.Draw("Preset " + (i + 1) + " (" + lightPreset.name + ")", lightPreset);

                    if (!fold) continue;

                    EditorGUI.indentLevel++;

                    lightPreset.name = EditorGUILayout.TextField("Name", lightPreset.name);

                    EditorGUILayout.Space();

                    DrawLightLayers(lightPreset.layerSetting);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }

        public class EventPresets
        {
            public static void Draw(EventPresetList eventPresetList)
            {
                var foldout = GUIFoldoutHeader.Begin("Light Event Presets (" + (eventPresetList.list.Length - 1) + ")",
                    eventPresetList);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                var bufferCount = EditorGUILayout.IntSlider("Count", eventPresetList.list.Length - 1, 1, 4) + 1;

                if (bufferCount != eventPresetList.list.Length)
                {
                    var oldCount = eventPresetList.list.Length;

                    Array.Resize(ref eventPresetList.list, bufferCount);

                    for (var i = oldCount; i < bufferCount; i++) eventPresetList.list[i] = new EventPreset(i);
                }

                for (var i = 1; i < eventPresetList.list.Length; i++)
                {
                    var eventPreset = eventPresetList.list[i];

                    var fold = GUIFoldout.Draw("Preset " + i + " (" + eventPreset.name + ")", eventPreset);

                    if (!fold) continue;

                    EditorGUI.indentLevel++;

                    eventPreset.name = EditorGUILayout.TextField("Name", eventPreset.name);

                    EditorGUILayout.Space();

                    DrawEventLayers(eventPreset.layerSetting);

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }

        public class LightmapPresets
        {
            public static void Draw(LightmapPresetList lightmapList)
            {
                var foldout =
                    GUIFoldoutHeader.Begin("Lightmap Presets (" + lightmapList.list.Length + ")", lightmapList);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                var bufferCount = EditorGUILayout.IntSlider("Count", lightmapList.list.Length, 1, 8);

                if (bufferCount != lightmapList.list.Length)
                {
                    var oldCount = lightmapList.list.Length;

                    Array.Resize(ref lightmapList.list, bufferCount);

                    for (var i = oldCount; i < bufferCount; i++) lightmapList.list[i] = new LightmapPreset(i);
                }

                for (var i = 0; i < lightmapList.list.Length; i++)
                {
                    var lightmapPreset = lightmapList.list[i];

                    var fold = GUIFoldout.Draw("Preset " + (i + 1) + " (" + lightmapPreset.name + ")", lightmapPreset);

                    if (!fold) continue;

                    EditorGUI.indentLevel++;

                    lightmapPreset.name = EditorGUILayout.TextField("Name", lightmapPreset.name);

                    EditorGUILayout.Space();

                    lightmapPreset.type = (LightmapPreset.Type)EditorGUILayout.EnumPopup("Type", lightmapPreset.type);

                    lightmapPreset.hdr = (LightmapPreset.HDR)EditorGUILayout.EnumPopup("HDR", lightmapPreset.hdr);

                    switch (lightmapPreset.type)
                    {
                        case LightmapPreset.Type.RGB24:
                        case LightmapPreset.Type.R8:
                        case LightmapPreset.Type.RHalf:

                            EditorGUILayout.Space();

                            CommonSettings(lightmapPreset);

                            EditorGUILayout.Space();

                            EditorGUILayout.Space();

                            LayerSettings.DrawList(lightmapPreset.dayLayers,
                                "Day Layers (" + lightmapPreset.dayLayers.list.Length + ")",
                                Lighting2D.Profile.layers.dayLayers, true);

                            EditorGUILayout.Space();

                            LayerSettings.DrawList(lightmapPreset.lightLayers,
                                "Light Layers (" + lightmapPreset.lightLayers.list.Length + ")",
                                Lighting2D.Profile.layers.lightLayers, false);

                            EditorGUILayout.Space();

                            break;

                        case LightmapPreset.Type.Depth8:

                            lightmapPreset.depth = EditorGUILayout.IntSlider("Depth", lightmapPreset.depth, -100, 100);

                            lightmapPreset.resolution =
                                EditorGUILayout.Slider("Resolution", lightmapPreset.resolution, 0.25f, 1.0f);

                            EditorGUILayout.Space();

                            LayerSettings.DrawList(lightmapPreset.dayLayers,
                                "Day Layers (" + lightmapPreset.dayLayers.list.Length + ")",
                                Lighting2D.Profile.layers.dayLayers, false);

                            EditorGUILayout.Space();

                            break;
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }

        public class Layers
        {
            public static void Draw(Profile profile)
            {
                var foldout = GUIFoldoutHeader.Begin("Layers", profile.layers);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                DrawList(profile.layers.colliderLayers, "Collider Layers", "Collider Layer");

                EditorGUILayout.Space();

                DrawList(profile.layers.lightLayers, "Light Layers", "Light Layer");

                EditorGUILayout.Space();

                DrawList(profile.layers.dayLayers, "Day Layers", "Day Layer");

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }

            public static void DrawList(LayersList layerList, string name, string singular)
            {
                var foldout = GUIFoldout.Draw(name, layerList);

                if (!foldout) return;

                EditorGUI.indentLevel++;

                var lightLayerCount = EditorGUILayout.IntSlider("Count", layerList.names.Length, 1, 10);

                if (lightLayerCount != layerList.names.Length)
                {
                    var oldCount = layerList.names.Length;

                    Array.Resize(ref layerList.names, lightLayerCount);

                    for (var i = oldCount; i < lightLayerCount; i++) layerList.names[i] = singular + " " + i;
                }

                for (var i = 0; i < lightLayerCount; i++)
                    layerList.names[i] = EditorGUILayout.TextField(" ", layerList.names[i]);

                EditorGUI.indentLevel--;
            }
        }

        public class QualitySettings
        {
            public static void Draw(Profile profile)
            {
                var foldout = GUIFoldoutHeader.Begin("Quality", profile.qualitySettings);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                profile.qualitySettings.projection =
                    (Projection)EditorGUILayout.EnumPopup("Projection", profile.qualitySettings.projection);

                profile.qualitySettings.coreAxis =
                    (CoreAxis)EditorGUILayout.EnumPopup("Core Axis", profile.qualitySettings.coreAxis);

                profile.qualitySettings.updateMethod =
                    (UpdateMethod)EditorGUILayout.EnumPopup("Update Method", profile.qualitySettings.updateMethod);

                profile.qualitySettings.lightTextureSize = (LightingSourceTextureSize)EditorGUILayout.Popup(
                    "Light Resolution", (int)profile.qualitySettings.lightTextureSize,
                    LightingSettings.QualitySettings.LightingSourceTextureSizeArray);

                profile.qualitySettings.lightFilterMode =
                    (FilterMode)EditorGUILayout.EnumPopup("Light Filter Mode", profile.qualitySettings.lightFilterMode);

                profile.qualitySettings.lightEffectTextureSize = (LightingSourceTextureSize)EditorGUILayout.Popup(
                    "Translucent Resolution", (int)profile.qualitySettings.lightEffectTextureSize,
                    LightingSettings.QualitySettings.LightingSourceTextureSizeArray);

                profile.qualitySettings.lightmapFilterMode =
                    (FilterMode)EditorGUILayout.EnumPopup("Lightmap Filter Mode",
                        profile.qualitySettings.lightmapFilterMode);

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }
        }

        public class DayLighting
        {
            public static void Draw(Profile profile)
            {
                var foldout = GUIFoldoutHeader.Begin("Day Lighting", profile.dayLightingSettings);

                if (!foldout)
                {
                    GUIFoldoutHeader.End();
                    return;
                }

                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                profile.dayLightingSettings.ShadowColor =
                    EditorGUILayout.ColorField("Shadow Color", profile.dayLightingSettings.ShadowColor);

                profile.dayLightingSettings.ShadowColor.a = EditorGUILayout.Slider("Shadow Alpha",
                    profile.dayLightingSettings.ShadowColor.a, 0, 1);

                profile.dayLightingSettings.direction =
                    EditorGUILayout.Slider("Direction", profile.dayLightingSettings.direction, 0, 360);

                profile.dayLightingSettings.height =
                    EditorGUILayout.Slider("Height", profile.dayLightingSettings.height, 0.1f, 10);

                NormalMap.Draw(profile);

                EditorGUI.indentLevel--;

                GUIFoldoutHeader.End();
            }

            public class NormalMap
            {
                public static void Draw(Profile profile)
                {
                    profile.dayLightingSettings.bumpMap.height = EditorGUILayout.Slider("Bump Height",
                        profile.dayLightingSettings.bumpMap.height, 0, 5);
                    profile.dayLightingSettings.bumpMap.strength = EditorGUILayout.Slider("Bump Strength",
                        profile.dayLightingSettings.bumpMap.strength, 0, 5);
                }
            }
        }

        public class LayerSettings
        {
            public static void DrawList(LightmapLayerList lightmapLayers, string name, LayersList layerList,
                bool drawType)
            {
                var foldout = GUIFoldout.Draw(name, lightmapLayers);

                if (!foldout) return;

                EditorGUI.indentLevel++;

                var layerSettings = lightmapLayers.Get();

                var layerCount = EditorGUILayout.IntSlider("Count", layerSettings.Length, 0, 10);

                EditorGUILayout.Space();

                if (layerCount != layerSettings.Length)
                {
                    var oldCount = layerSettings.Length;

                    Array.Resize(ref layerSettings, layerCount);

                    for (var i = oldCount; i < layerCount; i++)
                        if (layerSettings[i] == null)
                        {
                            layerSettings[i] = new LightmapLayer();
                            layerSettings[i].id = i;
                        }

                    lightmapLayers.SetArray(layerSettings);
                }

                for (var i = 0; i < layerSettings.Length; i++)
                {
                    layerSettings[i].id = EditorGUILayout.Popup("Layer", layerSettings[i].id, layerList.GetNames());

                    if (drawType)
                    {
                        layerSettings[i].type = (LayerType)EditorGUILayout.EnumPopup("Type", layerSettings[i].type);
                        layerSettings[i].sorting =
                            (LayerSorting)EditorGUILayout.EnumPopup("Sorting", layerSettings[i].sorting);
                    }

                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}