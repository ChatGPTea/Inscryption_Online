/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIToolbar : IMGUIView
    {
        BindableProperty<int> IndexProperty { get; }
        IMGUIToolbar Menus(List<string> menuNames);
        IMGUIToolbar AddMenu(string name, Action<string> onMenuSelected = null);

        IMGUIToolbar Index(int index);
    }

    internal class IMGUIIMGUIToolbarView : IMGUIAbstractView, IMGUIToolbar
    {
        private List<string> MenuNames = new();

        private List<Action<string>> MenuSelected = new();

        public IMGUIIMGUIToolbarView()
        {
            IndexProperty = new BindableProperty<int>();

            IndexProperty.Register(index =>
            {
                if (MenuSelected.Count > index) MenuSelected[index].Invoke(MenuNames[index]);
            });

            Style = new FluentGUIStyle(() => GUI.skin.button);
        }

        public IMGUIToolbar Menus(List<string> menuNames)
        {
            MenuNames = menuNames;
            // empty
            MenuSelected = MenuNames.Select(menuName => new Action<string>(str => { })).ToList();
            return this;
        }

        public IMGUIToolbar AddMenu(string name, Action<string> onMenuSelected = null)
        {
            MenuNames.Add(name);
            if (onMenuSelected == null)
                MenuSelected.Add(item => { });
            else
                MenuSelected.Add(onMenuSelected);

            return this;
        }

        public BindableProperty<int> IndexProperty { get; }

        public IMGUIToolbar Index(int index)
        {
            IndexProperty.Value = index;
            return this;
        }

        protected override void OnGUI()
        {
            IndexProperty.Value =
                GUILayout.Toolbar(IndexProperty.Value, MenuNames.ToArray(), Style.Value, LayoutStyles);
        }
    }
}
#endif