/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public class RenderEndCommandExecutor
    {
        // 全局的
        private static readonly RenderEndCommandExecutor mGlobal = new();

        private Queue<Action> mCommands { get; } = new();

        public static void PushCommand(Action command)
        {
            mGlobal.Push(command);
        }

        public static void ExecuteCommand()
        {
            mGlobal.Execute();
        }

        public void Push(Action command)
        {
            mCommands.Enqueue(command);
        }

        public void Execute()
        {
            while (mCommands.Count > 0) mCommands.Dequeue().Invoke();
        }
    }
}