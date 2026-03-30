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
using UnityEditor;

namespace QFramework
{
    public interface IPopup : IMGUIView
    {
        BindableProperty<int> IndexProperty { get; }
        IPopup WithIndexAndMenus(int index, params string[] menus);

        IPopup OnIndexChanged(Action<int> indexChanged);

        IPopup ToolbarStyle();
        IPopup Menus(List<string> value);
    }

    public class PopupView : IMGUIAbstractView, IPopup
    {
        private string[] mMenus = { };

        protected PopupView()
        {
            mStyle = new FluentGUIStyle(() => EditorStyles.popup);
        }

        public BindableProperty<int> IndexProperty { get; } = new();

        public IPopup Menus(List<string> menus)
        {
            mMenus = menus.ToArray();
            return this;
        }

        public IPopup WithIndexAndMenus(int index, params string[] menus)
        {
            IndexProperty.Value = index;
            mMenus = menus;
            return this;
        }

        public IPopup OnIndexChanged(Action<int> indexChanged)
        {
            IndexProperty.Register(indexChanged);
            return this;
        }

        public IPopup ToolbarStyle()
        {
            mStyle = new FluentGUIStyle(() => EditorStyles.toolbarPopup);
            return this;
        }

        public static IPopup Create()
        {
            return new PopupView();
        }

        protected override void OnGUI()
        {
            IndexProperty.Value =
                EditorGUILayout.Popup(IndexProperty.Value, mMenus, mStyle.Value, LayoutStyles);
        }
    }
}
#endif