/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Match
{
    public class DefaultMatchConfig : ScriptableObject
    {
        private static DefaultMatchConfig _instance;

        public MatchConfig MatchConfig;

        public static DefaultMatchConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    var result = AssetDatabase.FindAssets($"t:{nameof(DefaultMatchConfig)}");
                    if (result.Length <= 0)
                        Debug.LogError("DefaultMatchConfig not found");
                    else
                        _instance = AssetDatabase.LoadAssetAtPath<DefaultMatchConfig>(
                            AssetDatabase.GUIDToAssetPath(result[0]));
                }

                return _instance;
            }
        }
    }
}

#endif