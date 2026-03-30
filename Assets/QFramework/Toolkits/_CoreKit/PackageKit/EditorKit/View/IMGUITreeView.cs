/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

namespace QFramework
{
    public class TreeNode : VerticalLayout
    {
        private readonly IMGUIHorizontalLayout mFirstLine = EasyIMGUI.Horizontal();

        public string Content;

        private readonly VerticalLayout mSpreadView = new();
        public BindableProperty<bool> Spread;

        public TreeNode(bool spread, string content, int indent = 0, bool autosaveSpreadState = false)
        {
            if (autosaveSpreadState) spread = EditorPrefs.GetBool(content, spread);

            Content = content;
            Spread = new BindableProperty<bool>(spread);

            Style = new FluentGUIStyle(() => EditorStyles.foldout);

            mFirstLine.Parent(this);
            mFirstLine.AddChild(EasyIMGUI.Space().Pixel(indent));

            if (autosaveSpreadState) Spread.Register(value => EditorPrefs.SetBool(content, value));


            EasyIMGUI.Custom().OnGUI(() =>
                {
                    Spread.Value = EditorGUILayout.Foldout(Spread.Value, Content, true, Style.Value);
                })
                .Parent(mFirstLine);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                if (Spread.Value) mSpreadView.DrawGUI();
            }).Parent(this);
        }

        public TreeNode Add2FirstLine(IMGUIView view)
        {
            view.Parent(mFirstLine);
            return this;
        }

        public TreeNode FirstLineBox()
        {
            mFirstLine.Box();

            return this;
        }

        public TreeNode SpreadBox()
        {
            mSpreadView.VerticalStyle = "box";

            return this;
        }

        public TreeNode Add2Spread(IMGUIView view)
        {
            view.Parent(mSpreadView);
            return this;
        }
    }
}
#endif