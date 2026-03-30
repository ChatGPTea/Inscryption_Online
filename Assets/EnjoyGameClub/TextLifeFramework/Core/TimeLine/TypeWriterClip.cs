/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnjoyGameClub.TextLifeFramework.Core.TimeLine
{
    public class TypeWriterClip : PlayableAsset
    {
        public bool KeepText;
        public string content;
        public bool AllowFixedTime;
        public float FixedPerCharTime = 0.2f;

        [HideInInspector] public TMP_Text Text;
        [HideInInspector] public TimelineClip Clip;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TypeWriterBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Clip = this;
            Clip.displayName = content;
            return playable;
        }
    }
}