/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using EnjoyGameClub.TextLifeFramework.Core.SerializedObject;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.LiteMotion
{
    [CreateAssetMenu(fileName = "LiteMotion", menuName = "TextLife/Animation/LiteMotion")]
    public class LiteMotion : AnimationProcess
    {
        public SerializedObjectList<LiteMotionBehaviour> characterBehaviours = new();

        protected override Character OnProgress(Character character)
        {
            if (characterBehaviours.List == null) return character;

            foreach (var variableDataBehaviour in characterBehaviours.List) variableDataBehaviour.Execute(character);

            return character;
        }
    }
}