/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEditor;

#if UNITY_EDITOR

namespace QFramework
{
    public interface IMGUIEnumPopup : IMGUIView
    {
        // IPopup WithIndexAndMenus(int index, params string[] menus);

        // IPopup OnIndexChanged(Action<int> indexChanged);

        // IPopup ToolbarStyle();

        BindableProperty<Enum> ValueProperty { get; }
        // IPopup Menus(List<string> value);
    }

    public class IMGUIEnumPopupView : IMGUIAbstractView, IMGUIEnumPopup
    {
        public IMGUIEnumPopupView(Enum initValue)
        {
            ValueProperty = new BindableProperty<Enum>(initValue);
            ValueProperty.Value = initValue;
            Style = new FluentGUIStyle(() => EditorStyles.popup);
        }

        public BindableProperty<Enum> ValueProperty { get; }

        protected override void OnGUI()
        {
            var enumType = ValueProperty.Value;
            ValueProperty.Value = EditorGUILayout.EnumPopup(enumType, Style.Value, LayoutStyles);
        }
    }
}
#endif