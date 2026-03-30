/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public abstract class Pool<T> : IPool<T>
    {
        /// <summary>
        ///     存储相关数据的栈
        /// </summary>
        protected readonly Stack<T> mCacheStack = new();

        protected IObjectFactory<T> mFactory;

        /// <summary>
        ///     default is 5
        /// </summary>
        protected int mMaxCount = 12;

        #region ICountObserverable

        /// <summary>
        ///     Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurCount => mCacheStack.Count;

        #endregion

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0
                ? mFactory.Create()
                : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);

        public void SetObjectFactory(IObjectFactory<T> factory)
        {
            mFactory = factory;
        }

        public void SetFactoryMethod(Func<T> factoryMethod)
        {
            mFactory = new CustomObjectFactory<T>(factoryMethod);
        }

        public void Clear(Action<T> onClearItem = null)
        {
            if (onClearItem != null)
                foreach (var poolObject in mCacheStack)
                    onClearItem(poolObject);

            mCacheStack.Clear();
        }
    }
}