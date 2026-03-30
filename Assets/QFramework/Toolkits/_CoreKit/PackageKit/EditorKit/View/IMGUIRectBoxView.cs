/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIRectBox : IMGUIView, IHasRect<IMGUIRectBox>
    {
        IMGUIRectBox Text(string text);
    }

    public class IMGUIRectBoxView : IMGUIAbstractView, IMGUIRectBox
    {
        private Rect mRect = new(0, 0, 200, 100);

        private string mText = string.Empty;

        public IMGUIRectBoxView()
        {
            mStyle = new FluentGUIStyle(() => GUI.skin.box);
        }

        public IMGUIRectBox Text(string labelText)
        {
            mText = labelText;

            return this;
        }

        public IMGUIRectBox Rect(Rect rect)
        {
            mRect = rect;
            return this;
        }

        public IMGUIRectBox Position(Vector2 position)
        {
            mRect.position = position;
            return this;
        }

        public IMGUIRectBox Position(float x, float y)
        {
            mRect.x = x;
            mRect.y = y;
            return this;
        }

        public IMGUIRectBox Size(float width, float height)
        {
            mRect.width = width;
            mRect.height = height;
            return this;
        }

        public IMGUIRectBox Size(Vector2 size)
        {
            mRect.size = size;
            return this;
        }

        public IMGUIRectBox Width(float width)
        {
            mRect.width = width;
            return this;
        }

        public IMGUIRectBox Height(float height)
        {
            mRect.height = height;
            return this;
        }

        protected override void OnGUI()
        {
            GUI.Box(mRect, mText, mStyle.Value);
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            throw new NotImplementedException();
        }
    }
}