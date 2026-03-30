/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Xml;
using UnityEngine;

namespace QFramework
{
    public interface IMGUILabel : IMGUIView, IHasText<IMGUILabel>, IHasTextGetter<IMGUILabel>, IXMLToObjectConverter
    {
    }

    internal class IMGUILabelView : IMGUIAbstractView, IMGUILabel
    {
        private readonly FluentGUIStyle mFluentGUIStyle;

        private Func<string> mTextGetter;

        public IMGUILabelView()
        {
            mFluentGUIStyle = FluentGUIStyle.Label();
        }

        public string Content { get; set; }

        public IMGUILabel Text(string labelText)
        {
            Content = labelText;
            return this;
        }

        public IMGUILabel Text(Func<string> textGetter)
        {
            mTextGetter = textGetter;
            return this;
        }

        public T Convert<T>(XmlNode node) where T : class
        {
            var label = EasyIMGUI.Label();

            foreach (XmlAttribute childNodeAttribute in node.Attributes)
                if (childNodeAttribute.Name == "Id")
                    label.Id = childNodeAttribute.Value;
                else if (childNodeAttribute.Name == "Text")
                    label.Text(childNodeAttribute.Value);
                else if (childNodeAttribute.Name == "FontBold")
                    label.FontBold();
                else if (childNodeAttribute.Name == "FontSize") label.FontSize(int.Parse(childNodeAttribute.Value));

            return label as T;
        }

        protected override void OnGUI()
        {
            GUILayout.Label(mTextGetter == null ? Content : mTextGetter(), mFluentGUIStyle.Value, LayoutStyles);
        }
    }
}
#endif