/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Threading.Tasks;

namespace QFramework
{
    public class TaskAction : IAction
    {
        private static readonly SimpleObjectPool<TaskAction> mPool = new(() => new TaskAction(), null, 10);

        private Task mExecutingTask;

        private Func<Task> mTaskGetter;

        private TaskAction()
        {
        }

        public bool Paused { get; set; }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mTaskGetter = null;
                if (mExecutingTask != null)
                {
                    mExecutingTask.Dispose();
                    mExecutingTask = null;
                }

                ActionQueue.AddCallback(new ActionQueueRecycleCallback<TaskAction>(mPool, this));
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
            StartTask();
        }

        public void OnExecute(float dt)
        {
        }

        public void OnFinish()
        {
        }

        public static TaskAction Allocate(Func<Task> taskGetter)
        {
            var coroutineAction = mPool.Allocate();
            coroutineAction.ActionID = ActionKit.ID_GENERATOR++;
            coroutineAction.Deinited = false;
            coroutineAction.Reset();
            coroutineAction.mTaskGetter = taskGetter;
            return coroutineAction;
        }

        private async void StartTask()
        {
            mExecutingTask = mTaskGetter();
            await mExecutingTask;
            Status = ActionStatus.Finished;
            mExecutingTask = null;
        }
    }

    public static class TaskExtension
    {
        public static ISequence Task(this ISequence self, Func<Task> taskGetter)
        {
            return self.Append(TaskAction.Allocate(taskGetter));
        }

        public static IAction ToAction(this Task self)
        {
            return TaskAction.Allocate(() => self);
        }
    }
}