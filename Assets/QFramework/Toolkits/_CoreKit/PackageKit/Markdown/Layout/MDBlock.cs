/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEngine;

namespace QFramework
{
    internal abstract class MDBlock
    {
        public string ID = null;
        public float Indent;
        public MDBlock Parent = null;
        public Rect Rect = new();

        public MDBlock(float indent)
        {
            Indent = indent;
        }

        public abstract void Arrange(MDContext context, Vector2 anchor, float maxWidth);
        public abstract void Draw(MDContext context);

        public virtual MDBlock Find(string id)
        {
            return id.Equals(ID, StringComparison.Ordinal) ? this : null;
        }
    }
}
#endif