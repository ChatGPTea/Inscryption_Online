/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject
{
    public class PersistentScriptableObject : ScriptableObject, IScriptableObjectSaveable, IScriptableObjectCloneable
    {
        public object Clone()
        {
            var cloneObject = CreateInstance(GetType());
            var fieldInfos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var variableFieldInfo in fieldInfos)
                variableFieldInfo.SetValue(cloneObject,
                    typeof(IScriptableObjectCloneable).IsAssignableFrom(variableFieldInfo.FieldType)
                        ? (variableFieldInfo.GetValue(this) as IScriptableObjectCloneable).Clone()
                        : variableFieldInfo.GetValue(this));

            return cloneObject;
        }

        public object Save(bool isRootAsset = false, object rootObject = null, string rootPath = "")
        {
#if UNITY_EDITOR


            if (isRootAsset)
            {
                var path = EditorUtility.SaveFilePanel(
                    "Save ScriptableObject", // 窗口标题
                    "Assets", // 默认路径（可以是任何目录）
                    "NewScriptableObject", // 默认文件名
                    "asset" // 文件类型过滤器（只保存 .asset 文件）
                );
                // 如果用户没有选择路径，返回
                if (string.IsNullOrEmpty(path)) return null;

                // 如果路径不是以 "Assets" 开头，修改为保存到 Unity 的 Assets 文件夹
                if (!path.StartsWith("Assets")) path = "Assets/" + path.Split('/')[^1];

                rootObject = Instantiate(this);
                rootPath = path;
                AssetDatabase.CreateAsset((Object)rootObject, path);
                AssetDatabase.SaveAssets();
            }
            else
            {
                // Save sub scriptable asset.
                rootObject = Instantiate(this);
                ((Object)rootObject).name = GetType().Name;
                AssetDatabase.AddObjectToAsset((Object)rootObject, rootPath);
                AssetDatabase.SaveAssets();
            }

            // Check fields.
            var fieldInfos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var variableFieldInfo in fieldInfos)
                if (typeof(IScriptableObjectSaveable).IsAssignableFrom(variableFieldInfo.FieldType))
                {
                    var newValue =
                        (variableFieldInfo.GetValue(this) as IScriptableObjectSaveable)?.Save(false, rootObject,
                            rootPath);
                    variableFieldInfo.SetValue(rootObject, newValue);
                }

            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty((Object)rootObject);
#endif
            return rootObject;
        }
    }
}