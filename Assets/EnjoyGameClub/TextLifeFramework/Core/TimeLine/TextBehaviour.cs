/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using TMPro;
using UnityEngine.Playables;

namespace EnjoyGameClub.TextLifeFramework.Core.TimeLine
{
    public class TextBehaviour : PlayableBehaviour
    {
        private bool _isOnFrame;
        private string _originContent;
        public string Content;
        public TMP_Text Text;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (_isOnFrame) return;

            _originContent = Text.text;
            Text.text = Content;
            _isOnFrame = true;
            base.OnBehaviourPlay(playable, info);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!_isOnFrame) return;
            Text.text = _originContent;
            _isOnFrame = false;
            base.OnBehaviourPause(playable, info);
        }
    }
}