/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public struct MDStyle
    {
        public static readonly MDStyle Default = new();

        private const int FlagBold = 0x0100;
        private const int FlagItalic = 0x0200;
        private const int FlagFixed = 0x0400;
        private const int FlagLink = 0x0800;
        private const int FlagBlock = 0x1000;

        private const int MaskSize = 0x000F;
        private const int MaskWeight = 0x0300;

        private int mStyle;

        public static bool operator ==(MDStyle a, MDStyle b)
        {
            return a.mStyle == b.mStyle;
        }

        public static bool operator !=(MDStyle a, MDStyle b)
        {
            return a.mStyle != b.mStyle;
        }

        public override bool Equals(object a)
        {
            return a is MDStyle ? ((MDStyle)a).mStyle == mStyle : false;
        }

        public override int GetHashCode()
        {
            return mStyle.GetHashCode();
        }


        public void Clear()
        {
            mStyle = 0x0000;
        }

        public bool Bold
        {
            get => (mStyle & FlagBold) != 0x0000;
            set
            {
                if (value) mStyle |= FlagBold;
                else mStyle &= ~FlagBold;
            }
        }

        public bool Italic
        {
            get => (mStyle & FlagItalic) != 0x0000;
            set
            {
                if (value) mStyle |= FlagItalic;
                else mStyle &= ~FlagItalic;
            }
        }

        public bool Fixed
        {
            get => (mStyle & FlagFixed) != 0x0000;
            set
            {
                if (value) mStyle |= FlagFixed;
                else mStyle &= ~FlagFixed;
            }
        }

        public bool Link
        {
            get => (mStyle & FlagLink) != 0x0000;
            set
            {
                if (value) mStyle |= FlagLink;
                else mStyle &= ~FlagLink;
            }
        }

        public bool Block
        {
            get => (mStyle & FlagBlock) != 0x0000;
            set
            {
                if (value) mStyle |= FlagBlock;
                else mStyle &= ~FlagBlock;
            }
        }

        public int Size
        {
            get => mStyle & MaskSize;
            set => mStyle = (mStyle & ~MaskSize) | Mathf.Clamp(value, 0, 6);
        }

        public FontStyle GetFontStyle()
        {
            switch (mStyle & MaskWeight)
            {
                case FlagBold: return FontStyle.Bold;
                case FlagItalic: return FontStyle.Italic;
                case FlagBold | FlagItalic: return FontStyle.BoldAndItalic;
                default: return FontStyle.Normal;
            }
        }
    }
}