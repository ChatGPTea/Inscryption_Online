/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public interface IRepeat : ISequence
    {
    }

    public class Repeat : IRepeat
    {
        private static readonly SimpleObjectPool<Repeat> mSimpleObjectPool = new(() => new Repeat(), null, 5);

        private int mCurrentRepeatCount;

        private int mRepeatCount = -1;
        private Sequence mSequence;

        private Repeat()
        {
        }

        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            mCurrentRepeatCount = 0;
        }

        public void OnExecute(float dt)
        {
            if (mRepeatCount == -1 || mRepeatCount == 0)
            {
                if (mSequence.Execute(dt)) mSequence.Reset();
            }
            else if (mCurrentRepeatCount < mRepeatCount)
            {
                if (mSequence.Execute(dt))
                {
                    mCurrentRepeatCount++;

                    if (mCurrentRepeatCount >= mRepeatCount)
                        this.Finish();
                    else
                        mSequence.Reset();
                }
            }
        }

        public void OnFinish()
        {
        }

        public ISequence Append(IAction action)
        {
            mSequence.Append(action);
            return this;
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;

                mSequence.Deinit();

                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Repeat>(mSimpleObjectPool, this));
            }
        }

        public void Reset()
        {
            mCurrentRepeatCount = 0;
            Status = ActionStatus.NotStart;
            Paused = false;
            mSequence.Reset();
        }

        public static Repeat Allocate(int repeatCount = -1)
        {
            var repeat = mSimpleObjectPool.Allocate();
            repeat.ActionID = ActionKit.ID_GENERATOR++;
            repeat.mSequence = Sequence.Allocate();
            repeat.Deinited = false;
            repeat.Reset();
            repeat.mRepeatCount = repeatCount;
            return repeat;
        }
    }

    public static class RepeatExtension
    {
        public static ISequence Repeat(this ISequence self, Action<IRepeat> repeatSetting)
        {
            var repeat = QFramework.Repeat.Allocate();
            repeatSetting(repeat);
            return self.Append(repeat);
        }

        public static ISequence Repeat(this ISequence self, int repeatCount, Action<IRepeat> repeatSetting)
        {
            var repeat = QFramework.Repeat.Allocate(repeatCount);
            repeatSetting(repeat);
            return self.Append(repeat);
        }
    }
}