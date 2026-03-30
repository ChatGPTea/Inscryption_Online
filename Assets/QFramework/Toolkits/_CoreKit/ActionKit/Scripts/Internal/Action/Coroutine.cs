/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;

namespace QFramework
{
    internal class CoroutineAction : IAction
    {
        private static readonly SimpleObjectPool<CoroutineAction> mPool = new(() => new CoroutineAction(), null, 10);

        private Func<IEnumerator> mCoroutineGetter;

        private CoroutineAction()
        {
        }

        public bool Paused { get; set; }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mCoroutineGetter = null;

                ActionQueue.AddCallback(new ActionQueueRecycleCallback<CoroutineAction>(mPool, this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }

        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            ActionKitMonoBehaviourEvents.Instance.ExecuteCoroutine(mCoroutineGetter(),
                () => { Status = ActionStatus.Finished; });
        }

        public void OnExecute(float dt)
        {
        }

        public void OnFinish()
        {
        }

        public static CoroutineAction Allocate(Func<IEnumerator> coroutineGetter)
        {
            var coroutineAction = mPool.Allocate();
            coroutineAction.ActionID = ActionKit.ID_GENERATOR++;
            coroutineAction.Deinited = false;
            coroutineAction.Reset();
            coroutineAction.mCoroutineGetter = coroutineGetter;
            return coroutineAction;
        }
    }

    public static class CoroutineExtension
    {
        public static ISequence Coroutine(this ISequence self, Func<IEnumerator> coroutineGetter)
        {
            return self.Append(CoroutineAction.Allocate(coroutineGetter));
        }

        public static IAction ToAction(this IEnumerator self)
        {
            return CoroutineAction.Allocate(() => self);
        }
    }
}