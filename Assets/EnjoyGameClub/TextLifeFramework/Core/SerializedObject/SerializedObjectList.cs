/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject
{
    [Serializable]
    public class SerializedObjectList<T> : IScriptableObjectCloneable, IScriptableObjectSaveable
        where T : ScriptableObject
    {
        [SerializeReference] public List<T> List = new();

        public object Clone()
        {
            var cloneList = new SerializedObjectList<T>();
            foreach (var element in List)
            {
                if (element == null || !typeof(IScriptableObjectCloneable).IsAssignableFrom(element.GetType()))
                    continue;

                var newObject = (element as IScriptableObjectCloneable)?.Clone() as T;
                cloneList.List.Add(newObject);
            }

            return cloneList;
        }

        public object Save(bool isRootAsset = false, object rootObject = null, string rootPath = "")
        {
            var cloneList = new SerializedObjectList<T>();
            foreach (var element in List)
            {
                if (element == null || !typeof(IScriptableObjectSaveable).IsAssignableFrom(element.GetType())) continue;

                var newElement = (element as IScriptableObjectSaveable)?.Save(false, rootObject, rootPath);
                cloneList.List.Add(newElement as T);
            }
#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
            return cloneList;
        }
    }
}