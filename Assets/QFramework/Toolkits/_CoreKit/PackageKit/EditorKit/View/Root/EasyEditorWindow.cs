/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public abstract class EasyEditorWindow : EditorWindow, IMGUILayoutRoot
    {
        protected bool mInited;

        public bool Openning { get; set; }

        private void OnDestroy()
        {
            EditorApplication.update -= OnUpdate;
            Openning = false;
            OnClose();
        }

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
                return;
            }

            try
            {
                this.GetLayout().DrawGUI();
            }
#pragma warning disable CS0168
            catch (Exception _)
#pragma warning restore CS0168
            {
                GUIUtility.ExitGUI();
            }
        }

        VerticalLayout IMGUILayoutRoot.Layout { get; set; }
        RenderEndCommandExecutor IMGUILayoutRoot.RenderEndCommandExecutor { get; set; }

        public static T Create<T>(bool utility, string title = null, bool focused = true) where T : EasyEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title, focused);
        }


        public void Open()
        {
            Openning = true;
            EditorApplication.update += OnUpdate;
            Show();
        }

        public new void Close()
        {
            Openning = false;
            base.Close();
        }


        public void RemoveAllChildren()
        {
            this.GetLayout().Clear();
        }

        public abstract void OnClose();


        public abstract void OnUpdate();

        protected abstract void Init();

        public void ReInitNextFrame()
        {
            mInited = false;
        }
    }
}
#endif