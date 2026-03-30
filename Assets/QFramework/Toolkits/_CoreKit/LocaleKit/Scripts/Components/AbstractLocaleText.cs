/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public abstract class AbstractLocaleText : MonoBehaviour
    {
        public static Action<LocaleText> OnUpdateText;
        public bool SetTextOnInit = true;

        public List<LanguageText> LanguageTexts;

        private void Start()
        {
            LocaleKit.CurrentLanguage.Register(UpdateText)
                .UnRegisterWhenGameObjectDestroyed(gameObject);

            if (SetTextOnInit) UpdateText(LocaleKit.CurrentLanguage.Value);
        }

        protected abstract void SetText(string text);

        public void UpdateText(Language language)
        {
            SetText(LanguageTexts.First(lt => lt.Language == language).Text);
        }
    }
}