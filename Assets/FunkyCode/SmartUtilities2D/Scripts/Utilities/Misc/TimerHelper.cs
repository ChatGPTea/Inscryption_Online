using UnityEngine;

namespace FunkyCode.Utilities
{
    public class TimerHelper
    {
        public float time;

        public TimerHelper()
        {
            Reset();
        }

        public void Reset()
        {
            time = Time.realtimeSinceStartup;
        }

        public static TimerHelper Create()
        {
            return new TimerHelper();
        }

        public float GetMillisecs()
        {
            return (Time.realtimeSinceStartup - time) * 1000;
        }

        public float Get()
        {
            return Time.realtimeSinceStartup - time;
        }
    }
}