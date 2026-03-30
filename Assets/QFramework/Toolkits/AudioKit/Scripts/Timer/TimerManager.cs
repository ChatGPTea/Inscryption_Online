/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017~2024 liangxiegame UNDER MIT LICENSE
 * https://github.com/akbiggs/UnityTimer
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework.TimeExtend
{
    public class Timer
    {
        private const bool ShowLog = true;
        private static readonly List<Timer> timers = new();

        public static TimerDriver Driver; //拿驱动器的引用只是为了初始化驱动器

        /// <summary>
        ///     用户指定的定时器标志，便于手动清除、暂停、恢复
        /// </summary>
        private readonly string mFlag;

        private Action EndEvent;

        /// <summary>
        ///     缓存时间
        /// </summary>
        private float mCachedTime;

        /// <summary>
        ///     是否忽略Timescale
        /// </summary>
        private readonly bool mIgnorTimescale;

        /// <summary>
        ///     计时器是否结束
        /// </summary>
        private bool mIsFinish;

        /// <summary>
        ///     计时器是否暂停
        /// </summary>
        private bool mIsPause;

        /// <summary>
        ///     是否循环执行
        /// </summary>
        private readonly bool mLoop;

        /// <summary>
        ///     用户设定的定时时长
        /// </summary>
        private float mTime = -1;

        /// <summary>
        ///     已经流逝的时光
        /// </summary>
        private float mTimePassed;

        private Action<float> UpdateEvent;

        /// <summary>
        ///     构造定时器
        /// </summary>
        /// <param name="time">定时时长</param>
        /// <param name="flag">定时器标识符</param>
        /// <param name="loop">是否循环</param>
        /// <param name="ignorTimescale">是否忽略TimeScale</param>
        private Timer(float time, string flag, bool loop = false, bool ignorTimescale = true)
        {
            if (null == Driver) Driver = TimerDriver.Get; //初始化Time驱动
            mTime = time;
            mLoop = loop;
            mIgnorTimescale = ignorTimescale;
            mCachedTime = CurrentTime;
            if (timers.Exists(v => { return v.mFlag == flag; }))
                if (ShowLog)
                    Debug.LogWarningFormat("【TimerTrigger（容错）】:存在相同的标识符【{0}】！", flag);
            mFlag = string.IsNullOrEmpty(flag) ? GetHashCode().ToString() : flag; //设置辨识标志符
        }

        /// <summary>
        ///     获得当前时间
        /// </summary>
        private float CurrentTime => mIgnorTimescale ? Time.realtimeSinceStartup : Time.time;

        /// <summary>
        ///     刷新定时器
        /// </summary>
        private void Update()
        {
            if (!mIsFinish && !mIsPause) //运行中
            {
                mTimePassed = CurrentTime - mCachedTime;
                if (null != UpdateEvent) UpdateEvent.Invoke(Mathf.Clamp01(mTimePassed / mTime));
                if (mTimePassed >= mTime)
                {
                    if (null != EndEvent) EndEvent.Invoke();
                    if (mLoop)
                        mCachedTime = CurrentTime;
                    else
                        Stop();
                }
            }
        }

        /// <summary>
        ///     回收定时器
        /// </summary>
        private void Stop()
        {
            if (timers.Contains(this)) timers.Remove(this);
            mTime = -1;
            mIsFinish = true;
            mIsPause = false;
            UpdateEvent = null;
            EndEvent = null;
        }


        #region-------------UpdateAllTimer---------------

        public static void UpdateAllTimer()
        {
            for (var i = 0; i < timers.Count; i++)
                if (null != timers[i])
                    timers[i].Update();
        }

        #endregion
    }

    public class TimerDriver : MonoBehaviour
    {
        private void Update()
        {
            Timer.UpdateAllTimer();
        }

        #region 单例

        private static TimerDriver mInstance;

        public static TimerDriver Get
        {
            get
            {
                if (null == mInstance)
                    mInstance = FindObjectOfType<TimerDriver>() ??
                                new GameObject("TimerEntity").AddComponent<TimerDriver>();
                return mInstance;
            }
        }

        private void Awake()
        {
            mInstance = this;
        }

        #endregion
    }
}