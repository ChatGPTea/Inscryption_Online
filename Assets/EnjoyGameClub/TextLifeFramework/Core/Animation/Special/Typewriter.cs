/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using EnjoyGameClub.TextLifeFramework.Core.SerializedObject;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation.Special
{
    [CreateAssetMenu(fileName = "Type Writer", menuName = "TextLife/Animation/Type Writer")]
    [Serializable]
    public class Typewriter : AnimationProcess
    {
        public float AnimationSmooth = 0.8f;
        public SerializedObjectList<AnimationProcess> OnAppearProcess = new();
        public UnityEvent OnTextVisible;
        private float[] _characterAnimationTime = new float[1];
        private bool[] _characterVisibleState = new bool[1];
        private TMP_Text TMPText;

        protected override void OnStart()
        {
            foreach (var animationProcess in OnAppearProcess.List) animationProcess?.Start();

            _characterAnimationTime = new float[1];
            _characterVisibleState = new bool[1];
        }

        protected override void OnReset()
        {
            if (_characterAnimationTime == null) return;

            Array.Fill(_characterAnimationTime, 0, 0, _characterAnimationTime.Length);
            Array.Fill(_characterVisibleState, false, 0, _characterAnimationTime.Length);
            foreach (var animationProcess in OnAppearProcess.List) animationProcess.Start();
        }

        private void CheckData(Character character)
        {
            if (_characterAnimationTime == null || _characterAnimationTime.Length < character.TotalCount)
            {
                _characterAnimationTime = new float[character.TotalCount];
                _characterVisibleState = new bool[character.TotalCount];
                TMPText = character.TMPComponent;
            }
        }

        protected override Character OnProgress(Character character)
        {
            CheckData(character);
            // Calculate animation time
            var characterTime = character.Time.Time / AnimationSmooth;
            var normalizedTime = Mathf.Clamp01(characterTime);
            if (!_characterVisibleState[character.CharIndex] && normalizedTime > 0)
            {
                OnTextVisible?.Invoke();
                _characterVisibleState[character.CharIndex] = true;
            }

            if (_characterAnimationTime[character.CharIndex] >= 1) return character;


            _characterAnimationTime[character.CharIndex] = normalizedTime;


            // Appear effect processes
            foreach (var animationProcess in OnAppearProcess.List)
                animationProcess.Progress(_characterAnimationTime[character.CharIndex], normalizedTime,
                    character);

            return character;
        }

        /// <summary>
        ///     立即完成所有文本动画的展示。
        /// </summary>
        /// <remarks>
        ///     Immediately completes all text animations.
        /// </remarks>
        public void CompleteImmediately()
        {
            Array.Fill(_characterAnimationTime, 1, 0, _characterAnimationTime.Length);
            Array.Fill(_characterVisibleState, true, 0, _characterVisibleState.Length);
        }

        /// <summary>
        ///     显示指定文本并重置动画状态。
        /// </summary>
        /// <remarks>
        ///     Displays specified text and resets animation state.
        /// </remarks>
        /// <param name="text">要显示的文本</param>
        public void ShowText(string text)
        {
            TMPText.SetText(text);
            Reset();
        }
    }
}