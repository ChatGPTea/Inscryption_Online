/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using UnityEngine.Playables;

namespace EnjoyGameClub.TextLifeFramework.Core.TimeLine
{
    public class TypeWriterBehaviour : PlayableBehaviour
    {
        private double _duration;
        private int _lastPrintedCount;
        private bool _onTyping;
        private string _originContent;
        private double _perCharTime;
        private string _showContent;
        public TypeWriterClip Clip;


        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (_onTyping) return;

            _lastPrintedCount = 0;
            _originContent = Clip.Text.text;
            Clip.Text.text = "";
            _onTyping = true;
            _perCharTime = Clip.AllowFixedTime ? Clip.FixedPerCharTime : playable.GetDuration() / Clip.content.Length;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (string.IsNullOrEmpty(Clip.content))
                return;

            var currentTime = playable.GetTime();
            var currentPrintCount = (int)(currentTime / _perCharTime) + 1;
            if (currentPrintCount != _lastPrintedCount && currentPrintCount <= Clip.content.Length)
            {
                _showContent = Clip.content.Substring(0, currentPrintCount);
                Clip.Text.text = _showContent;
                _lastPrintedCount++;
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (!_onTyping) return;

            _lastPrintedCount = Clip.content.Length;
            Clip.Text.text = Clip.KeepText ? Clip.content : _originContent;
            _onTyping = false;
        }
    }
}