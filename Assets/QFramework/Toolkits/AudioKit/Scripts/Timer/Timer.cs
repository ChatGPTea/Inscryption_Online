/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2024 liangxiegame UNDER MIT LICENSE
 * https://github.com/akbiggs/UnityTimer
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("QFramework/ResKit/Timer")]
    public class Timer : MonoBehaviour, ISingleton
    {
        private readonly BinaryHeap<TimeItem> mScaleTimeHeap = new(128, BinaryHeapSortMode.kMin);

        private readonly BinaryHeap<TimeItem> mUnScaleTimeHeap = new(128, BinaryHeapSortMode.kMin);
        private float mCurrentUnScaleTime = -1;
        public static Timer Instance => MonoSingletonProperty<Timer>.Instance;

        public float CurrentScaleTime { get; private set; } = -1;

        public void Update()
        {
            UpdateMgr();
        }

        public void OnSingletonInit()
        {
            mUnScaleTimeHeap.Clear();
            mScaleTimeHeap.Clear();

            mCurrentUnScaleTime = Time.unscaledTime;
            CurrentScaleTime = Time.time;
        }

        #region 投递真实时间定时器

        //投递指定时间计时器：只支持标准时间

        public void Post2Really(TimeItem item)
        {
            item.SortScore = mCurrentUnScaleTime + item.DelayTime();
            mUnScaleTimeHeap.Insert(item);
        }

        #endregion

        public void UpdateMgr()
        {
            TimeItem item = null;
            mCurrentUnScaleTime = Time.unscaledTime;
            CurrentScaleTime = Time.time;

            #region 不受缩放影响定时器更新

            while ((item = mUnScaleTimeHeap.Top()) != null)
            {
                if (!item.IsEnable)
                {
                    mUnScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < mCurrentUnScaleTime)
                {
                    mUnScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.IsEnable && item.NeedRepeat())
                        Post2Really(item);
                    else
                        item.Recycle2Cache();
                }
                else
                {
                    break;
                }
            }

            #endregion

            #region 受缩放影响定时器更新

            while ((item = mScaleTimeHeap.Top()) != null)
            {
                if (!item.IsEnable)
                {
                    mScaleTimeHeap.Pop();
                    item.Recycle2Cache();
                    continue;
                }

                if (item.SortScore < CurrentScaleTime)
                {
                    mScaleTimeHeap.Pop();

                    item.OnTimeTick();

                    if (item.IsEnable && item.NeedRepeat())
                        Post2Scale(item);
                    else
                        item.Recycle2Cache();
                }
                else
                {
                    break;
                }
            }

            #endregion
        }

        #region 投递受缩放影响定时器

        public TimeItem Post2Scale(Action<int> callback, float delay, int repeat)
        {
            var item = TimeItem.Allocate(callback, delay, repeat);
            Post2Scale(item);
            return item;
        }

        public TimeItem Post2Scale(Action<int> callback, float delay)
        {
            var item = TimeItem.Allocate(callback, delay);
            Post2Scale(item);
            return item;
        }

        public void Post2Scale(TimeItem item)
        {
            item.SortScore = CurrentScaleTime + item.DelayTime();
            mScaleTimeHeap.Insert(item);
        }

        #endregion
    }
}