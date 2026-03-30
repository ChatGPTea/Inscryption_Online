/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace EnjoyGameClub.TextLifeFramework.Core
{
    /// <summary>
    ///     打字机组件，用于逐字显示文本内容。
    /// </summary>
    /// <remarks>
    ///     Typewriter component that displays text character by character.
    /// </remarks>
    public class TypeWriter : MonoBehaviour
    {
        /// <summary>
        ///     目标完整文本，将逐字显示。
        /// </summary>
        /// <remarks>
        ///     The full text to be displayed one character at a time.
        /// </remarks>
        [Header("Full Text")] public string FullText;

        /// <summary>
        ///     TextMeshPro 文本组件，用于显示内容。
        /// </summary>
        /// <remarks>
        ///     The TMP_Text component used to render the typing content.
        /// </remarks>
        [Header("Text Component")] public TMP_Text textComponent;

        /// <summary>
        ///     每个字符的显示间隔时间（秒）。
        /// </summary>
        /// <remarks>
        ///     Time interval (in seconds) between each character display.
        /// </remarks>
        [Header("Typing Settings")] public float typingInterval = 0.05f;

        /// <summary>
        ///     当文本发生改变时触发。
        /// </summary>
        /// <remarks>
        ///     Invoked when a new full text is set for typing.
        /// </remarks>
        [Header("Events")] public UnityEvent OnTextChanged;

        /// <summary>
        ///     每打印一个字符时触发，参数为当前已打印内容。
        /// </summary>
        /// <remarks>
        ///     Invoked each time a character is printed; provides current displayed text.
        /// </remarks>
        public UnityEvent<string> OnCharacterPrinted;

        /// <summary>
        ///     当所有字符打印完成时触发。
        /// </summary>
        /// <remarks>
        ///     Invoked when all characters have been printed.
        /// </remarks>
        public UnityEvent OnTypingCompleted;

        // 当前字符索引位置
        private int _charIndex = -1;

        // 用于构造当前显示的文本
        private readonly StringBuilder _displayedText = new();

        // 实际显示中的文本（缓存）
        private string _fullText;

        // 当前打字状态
        private TypingState _state;

        // 计时器，用于间隔字符输出
        private float _timer;

        /// <summary>
        ///     当前是否处于打字过程中。
        /// </summary>
        /// <remarks>
        ///     Indicates whether the typewriter is actively printing text.
        /// </remarks>
        public bool IsTyping => _state == TypingState.Typing;

        /// <summary>
        ///     Unity 初始化事件，自动绑定 TMP_Text。
        /// </summary>
        /// <remarks>
        ///     Unity Awake callback, auto-initializes TMP_Text component if not assigned.
        /// </remarks>
        private void Awake()
        {
            TryInitializeTextComponent();
        }

        /// <summary>
        ///     Unity 更新事件，处理打字逻辑。
        /// </summary>
        /// <remarks>
        ///     Unity Update loop to manage typing interval and update text.
        /// </remarks>
        private void Update()
        {
            CheckTextChanged();

            if (!IsTyping) return;

            _timer += Time.deltaTime;

            if (_timer >= typingInterval)
            {
                _timer -= typingInterval;
                PrintNextCharacter();
            }
        }

        /// <summary>
        ///     检查 FullText 是否发生变化，自动触发打字。
        /// </summary>
        /// <remarks>
        ///     Checks if FullText has changed and triggers the typing process if so.
        /// </remarks>
        private void CheckTextChanged()
        {
            if (FullText != null && _fullText != FullText)
            {
                _fullText = FullText;
                OnFullTextChanged();
            }
        }

        /// <summary>
        ///     初始化 TMP_Text 组件（如果未手动绑定）。
        /// </summary>
        /// <remarks>
        ///     Attempts to auto-assign TMP_Text component from the current GameObject.
        /// </remarks>
        private void TryInitializeTextComponent()
        {
            if (textComponent != null) return;

            if (TryGetComponent(out TMP_Text tmp))
                textComponent = tmp;
            else
                Debug.LogWarning("[TextLifeTypeWriter] TMP_Text component not found.");
        }

        /// <summary>
        ///     触发文本变化时的重置逻辑。
        /// </summary>
        /// <remarks>
        ///     Called when FullText changes; resets state and starts typing.
        /// </remarks>
        private void OnFullTextChanged()
        {
            _displayedText.Clear();
            _charIndex = -1;
            _state = TypingState.Typing;
            OnTextChanged?.Invoke();
        }

        /// <summary>
        ///     手动开始打印指定文本。
        /// </summary>
        /// <param name="text">要打印的完整文本</param>
        /// <remarks>
        ///     Starts typing with the specified full text.
        /// </remarks>
        public void StartTyping(string text)
        {
            FullText = text;
        }

        /// <summary>
        ///     打印下一个字符，并更新 UI 和事件。
        /// </summary>
        /// <remarks>
        ///     Appends the next character to the output and fires relevant events.
        /// </remarks>
        private void PrintNextCharacter()
        {
            if (_charIndex >= _fullText.Length - 1)
            {
                _state = TypingState.None;
                OnTypingCompleted?.Invoke();
                return;
            }

            _charIndex++;
            _displayedText.Append(_fullText[_charIndex]);
            textComponent.SetText(_displayedText.ToString());
            OnCharacterPrinted?.Invoke(_displayedText.ToString());
        }

        private enum TypingState
        {
            None,
            Typing
        }
    }
}