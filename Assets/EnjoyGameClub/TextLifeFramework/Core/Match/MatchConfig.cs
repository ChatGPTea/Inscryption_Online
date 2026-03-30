/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Match
{
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "TextLife/MatchConfig")]
    public class MatchConfig : ScriptableObject
    {
        [SerializeField] public List<MatchConfigData> MatchList = new();
    }

    [Serializable]
    public class MatchConfigData
    {
        public string matchTag;
        [SerializeField] public List<ScriptableObject> Process = new();
    }
}