/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using EnjoyGameClub.TextLifeFramework.Core.SerializedObject;

namespace EnjoyGameClub.TextLifeFramework.Core.Animation
{
    /// <summary>
    ///     动画处理的基类，用于编写TextLife框架中的文字动画过程。
    ///     提供了文字动画的时间控制和时间缩放功能。
    /// </summary>
    /// <remarks>
    ///     The base class for handling animation processes in the TextLife framework.
    ///     Provides timing and scaling functionalities for character animations.
    /// </remarks>
    [Serializable]
    public abstract class AnimationProcess : PersistentScriptableObject
    {
        /// <summary>
        ///     用于时间偏移计算的常量参数。
        /// </summary>
        /// <remarks>
        ///     Constant parameter used for time offset calculations.
        /// </remarks>
        protected const float TIME_OFFSET_PARAM = 0.001f;

        /// <summary>
        ///     是否启用动画处理。
        /// </summary>
        /// <remarks>
        ///     Determines whether the animation process is enabled.
        /// </remarks>
        public bool Enable = true;

        /// <summary>
        ///     时间缩放系数，用于控制动画的时间流逝速度。
        /// </summary>
        /// <remarks>
        ///     Scale factor for the animation time, controlling the speed of animation.
        /// </remarks>
        public float TimeScale = 1;

        /// <summary>
        ///     每个字符的时间偏移，用于控制字符的动画起始时间。
        /// </summary>
        /// <remarks>
        ///     Offset time applied per character in the animation sequence.
        /// </remarks>
        public float CharacterTimeOffset;

        /// <summary>
        ///     重置动画处理，设置重置时间并调用派生类的重置逻辑。
        /// </summary>
        /// <remarks>
        ///     Resets the animation process, sets the reset time, and calls the derived class reset logic.
        /// </remarks>
        public void Reset()
        {
            OnReset();
        }

        private void OnEnable()
        {
            Start();
        }

        /// <summary>
        ///     当文字发生改变时被调用一次，由TextLife进行调用。
        /// </summary>
        /// <remarks>
        ///     Called once when the text changes, by TextLife.
        /// </remarks>
        public void ChangedText()
        {
            OnTextChanged();
        }

        /// <summary>
        ///     启动动画处理流程，在启用脚本时调用。
        ///     派生类可以重写 OnStart 方法以实现初始化逻辑。
        /// </summary>
        /// <remarks>
        ///     Starts the animation process when the script is enabled.
        ///     Derived classes can override the OnStart method to implement initialization logic.
        /// </remarks>
        public void Start()
        {
            OnStart();
        }

        /// <summary>
        ///     派生类可以重写此方法以实现自定义的启动逻辑。
        /// </summary>
        /// <remarks>
        ///     The derived class can override this method to implement custom start logic.
        /// </remarks>
        protected virtual void OnStart()
        {
        }

        protected virtual void OnTextChanged()
        {
        }

        /// <summary>
        ///     派生类可以重写此方法来实现自定义的创建逻辑。
        /// </summary>
        /// <remarks>
        ///     The derived class can override this method to implement custom creation logic.
        /// </remarks>
        protected virtual void OnProcessCreate()
        {
        }

        /// <summary>
        ///     派生类可以重写此方法来实现自定义的重置逻辑。
        /// </summary>
        /// <remarks>
        ///     The derived class can override this method to implement custom reset logic.
        /// </remarks>
        protected virtual void OnReset()
        {
        }

        /// <summary>
        ///     进度更新函数，根据当前时间和字符信息更新动画进度。
        /// </summary>
        /// <param name="time">当前时间。</param>
        /// <param name="deltaTime">每帧的时间差。</param>
        /// <param name="character">当前处理的字符。</param>
        /// <returns>字符。</returns>
        /// <remarks>
        ///     Progress update function that updates the animation progress based on the current time and character information.
        /// </remarks>
        public void Progress(float time, float deltaTime, Character character)
        {
            if (!Enable) return;

            character.Time.Time = time;
            character.Time.Time *= TimeScale;
            character.Time.Time += character.CharIndex * -CharacterTimeOffset * TIME_OFFSET_PARAM;
            character.Time.DeltaTime = deltaTime * TimeScale;
            OnProgress(character);
        }

        /// <summary>
        ///     派生类可以重写此方法来处理动画进度更新。
        /// </summary>
        /// <param name="character">当前处理的字符。</param>
        /// <returns>更新后的字符。</returns>
        /// <remarks>
        ///     The derived class can override this method to handle animation progress updates.
        /// </remarks>
        protected virtual Character OnProgress(Character character)
        {
            return character;
        }
    }
}