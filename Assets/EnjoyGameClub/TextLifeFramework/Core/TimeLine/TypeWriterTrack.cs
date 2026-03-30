/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace EnjoyGameClub.TextLifeFramework.Core.TimeLine
{
    [TrackBindingType(typeof(TMP_Text))]
    [TrackClipType(typeof(TypeWriterClip))]
    [DisplayName("TextLife/TypeWriterTrack")]
    public class TypeWriterTrack : TrackAsset
    {
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            var director = graph.GetResolver() as PlayableDirector;
            var typeWriterClip = clip.asset as TypeWriterClip;
            typeWriterClip.Text = director.GetGenericBinding(this) as TMP_Text;
            typeWriterClip.Clip = clip;
            return base.CreatePlayable(graph, gameObject, clip);
        }
    }
}