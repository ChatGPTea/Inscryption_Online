/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public abstract class IMGUIAbstractView : IMGUIView
    {
        private bool mBeforeDrawCalled;

        private readonly Queue<Action> mCommands = new();


        private Color mPreviousBackgroundColor;


        protected FluentGUIStyle mStyle = new(() => new GUIStyle());
        private bool mVisible = true;

        private List<GUILayoutOption> mLayoutOptions { get; } = new();

        protected GUILayoutOption[] LayoutStyles { get; private set; }

        public string Id { get; set; }

        public bool Visible
        {
            get => VisibleCondition == null ? mVisible : VisibleCondition();
            set => mVisible = value;
        }

        public Func<bool> VisibleCondition { get; set; }

        public FluentGUIStyle Style
        {
            get => mStyle;
            protected set => mStyle = value;
        }

        public Color BackgroundColor { get; set; } = GUI.backgroundColor;

        public void RefreshNextFrame()
        {
            PushCommand(Refresh);
        }

        public void AddLayoutOption(GUILayoutOption option)
        {
            mLayoutOptions.Add(option);
        }

        public void Show()
        {
            Visible = true;
            OnShow();
        }

        public void Hide()
        {
            Visible = false;
            OnHide();
        }

        public void DrawGUI()
        {
            BeforeDraw();

            if (Visible)
            {
                mPreviousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = BackgroundColor;
                OnGUI();
                GUI.backgroundColor = mPreviousBackgroundColor;
            }

            if (mCommands.Count > 0) mCommands.Dequeue().Invoke();
        }

        public IMGUILayout Parent { get; set; }

        public void RemoveFromParent()
        {
            Parent.RemoveChild(this);
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        private void BeforeDraw()
        {
            if (!mBeforeDrawCalled)
            {
                OnBeforeDraw();

                LayoutStyles = mLayoutOptions.ToArray();

                mBeforeDrawCalled = true;
            }
        }

        protected virtual void OnBeforeDraw()
        {
        }

        protected virtual void OnRefresh()
        {
        }

        protected abstract void OnGUI();

        protected virtual void OnDisposed()
        {
        }
    }
}