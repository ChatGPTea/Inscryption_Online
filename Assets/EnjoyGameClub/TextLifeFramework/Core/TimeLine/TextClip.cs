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
    public class TextClip : PlayableAsset
    {
        [HideInInspector] public TMP_Text Text;

        [HideInInspector] public TimelineClip Clip;

        public string Content;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TextBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Text = Text;
            behaviour.Content = Content;
            Clip.displayName = Content;
            return playable;
        }
    }
}