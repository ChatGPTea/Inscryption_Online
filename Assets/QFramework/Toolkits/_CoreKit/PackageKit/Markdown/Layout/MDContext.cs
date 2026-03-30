/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework
{
    internal class MDContext
    {
        private readonly MDHandlerImages mImages;
        private readonly MDHandlerNavigate mNagivate;

        private readonly MDStyleConverter mStyleConverter;
        private GUIStyle mStyleGUI;

        public MDContext(GUISkin skin, MDHandlerImages images, MDHandlerNavigate navigate)
        {
            mStyleConverter = new MDStyleConverter(skin);
            mImages = images;
            mNagivate = navigate;

            Apply(MDStyle.Default);
        }

        public float LineHeight => mStyleGUI.lineHeight;

        public float MinWidth => LineHeight * 2.0f;

        public float IndentSize => LineHeight * 1.5f;

        public void SelectPage(string path)
        {
            mNagivate.SelectPage(path);
        }

        public Texture FetchImage(string url)
        {
            return mImages.FetchImage(url);
        }

        public void Reset()
        {
            Apply(MDStyle.Default);
        }

        public GUIStyle Apply(MDStyle style)
        {
            mStyleGUI = mStyleConverter.Apply(style);
            mStyleGUI.richText = true;
            return mStyleGUI;
        }

        public Vector2 CalcSize(GUIContent content)
        {
            return mStyleGUI.CalcSize(content);
        }
    }
}
#endif