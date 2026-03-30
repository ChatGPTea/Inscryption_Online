/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    internal class DelayFrame : IAction
    {
        private static readonly SimpleObjectPool<DelayFrame> mSimpleObjectPool = new(() => new DelayFrame(), null, 10);

        private int mDelayedFrameCount;

        private Action mOnDelayFinish;

        private int mStartFrameCount;
        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            mStartFrameCount = Time.frameCount;
        }

        public void OnExecute(float dt)
        {
            if (Time.frameCount >= mStartFrameCount + mDelayedFrameCount)
            {
                mOnDelayFinish?.Invoke();
                this.Finish();
            }
        }

        public void OnFinish()
        {
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mOnDelayFinish = null;
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<DelayFrame>(mSimpleObjectPool, this));
            }
        }

        public void Reset()
        {
            Status = ActionStatus.NotStart;
            Paused = false;
            mStartFrameCount = 0;
        }

        public static DelayFrame Allocate(int frameCount, Action onDelayFinish = null)
        {
            var delayFrame = mSimpleObjectPool.Allocate();
            delayFrame.ActionID = ActionKit.ID_GENERATOR++;
            delayFrame.Reset();
            delayFrame.Deinited = false;
            delayFrame.mDelayedFrameCount = frameCount;
            delayFrame.mOnDelayFinish = onDelayFinish;

            return delayFrame;
        }
    }

    public static class DelayFrameExtension
    {
        public static ISequence DelayFrame(this ISequence self, int frameCount)
        {
            return self.Append(QFramework.DelayFrame.Allocate(frameCount));
        }

        public static ISequence NextFrame(this ISequence self)
        {
            return self.Append(QFramework.DelayFrame.Allocate(1));
        }
    }
}