/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.Special
{
    [CreateAssetMenu(fileName = "Rainbow", menuName = "TextLife/Animation/Rainbow")]
    public class Rainbow : AnimationProcess
    {
        protected override void OnProcessCreate()
        {
            CharacterTimeOffset = 50;
            TimeScale = 0.1f;
        }

        protected override Character OnProgress(Character character)
        {
            var colorH1 = character.Time.Time < 0 ? character.Time.Time % 1 + 1 : character.Time.Time % 1;
            var colorH2 = character.Time.Time < 0
                ? (character.Time.Time - CharacterTimeOffset * TIME_OFFSET_PARAM * TimeScale) % 1 + 1
                : (character.Time.Time - CharacterTimeOffset * TIME_OFFSET_PARAM * TimeScale) % 1;
            var alpha = character.Color.VerticesColor[0].a;
            Color32 color1 = Color.HSVToRGB(colorH1, 1, 1);
            Color32 color2 = Color.HSVToRGB(colorH2, 1, 1);
            color1.a = alpha;
            color2.a = alpha;
            character.Color.VerticesColor[0] = color1;
            character.Color.VerticesColor[1] = color1;
            character.Color.VerticesColor[2] = color2;
            character.Color.VerticesColor[3] = color2;
            return character;
        }
    }
}