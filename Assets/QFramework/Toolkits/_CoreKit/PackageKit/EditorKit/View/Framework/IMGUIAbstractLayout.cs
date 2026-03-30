/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

namespace QFramework
{
    public abstract class IMGUIAbstractLayout : IMGUIAbstractView, IMGUILayout
    {
        protected List<IMGUIView> Children = new();

        public IMGUILayout AddChild(IMGUIView view)
        {
            Children.Add(view);
            view.Parent = this;
            return this;
        }

        public void RemoveChild(IMGUIView view)
        {
            PushCommand(() =>
            {
                Children.Remove(view);
                view.Parent = null;
            });

            view.Dispose();
        }

        public void Clear()
        {
            Children.ForEach(view =>
            {
                view.Parent = null;
                view.Dispose();
            });

            Children.Clear();
        }

        public override void Refresh()
        {
            Children.ForEach(view => view.Refresh());
            base.Refresh();
        }

        protected override void OnGUI()
        {
            OnGUIBegin();

            foreach (var child in Children) child.DrawGUI();

            OnGUIEnd();
        }

        protected virtual void OnGUIBegin()
        {
        }

        protected virtual void OnGUIEnd()
        {
        }
    }
}