/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace QFramework
{
    public static class AutoSaveDump
    {
        public static void PushSaveCommand<T>(this T self, Action action)
        {
            SaveCommandQueue.PushSaveCommand(typeof(T), action);
        }

        public static void Save<T>(this T self) where T : Object
        {
            EditorUtility.SetDirty(self);
            AssetDatabase.SaveAssets();
        }

        public class SaveCommandQueue
        {
            private static SaveCommandQueue mDefault;

            private readonly Dictionary<Type, SaveCommand> mCommandQueues = new();

            public List<Type> mType2Remove = new();

            public bool Started;


            public void Start()
            {
                EditorApplication.update += Update;
                Started = true;
            }

            private void Update()
            {
                foreach (var kv in mCommandQueues)
                    if (!kv.Value.Started)
                    {
                        kv.Value.Started = true;
                        kv.Value.StartTime = DateTime.Now;
                    }
                    else
                    {
                        if (kv.Value.StartTime + TimeSpan.FromSeconds(kv.Value.DelayTime) < DateTime.Now)
                        {
                            if (kv.Value != null)
                            {
                                kv.Value.Action?.Invoke();
                                kv.Value.finished = true;
                            }

                            mType2Remove.Add(kv.Key);
                        }
                    }

                foreach (var type in mType2Remove) mCommandQueues.Remove(type);

                mType2Remove.Clear();
            }

            public static void PushSaveCommand(Type t, Action action)
            {
                if (mDefault == null)
                {
                    mDefault = new SaveCommandQueue();
                    mDefault.Start();
                }
                else if (!mDefault.Started)
                {
                    mDefault.Start();
                }

                if (mDefault.mCommandQueues.ContainsKey(t))
                    mDefault.mCommandQueues[t] = new SaveCommand
                    {
                        Type = t,
                        Action = action,
                        DelayTime = 2
                    };
                else
                    mDefault.mCommandQueues.Add(t, new SaveCommand
                    {
                        Type = t,
                        Action = action,
                        DelayTime = 0.5f
                    });
            }

            private class SaveCommand
            {
                public Action Action;
                public float DelayTime;
                public bool finished;
                public bool Started;
                public DateTime StartTime;
                public Type Type;
            }
        }
    }
}
#endif