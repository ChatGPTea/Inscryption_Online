using System;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
namespace QFramework
{
    // https://gist.github.com/benblo/10732554
    public class EditorCoroutine
    {
        private readonly IEnumerator mRoutine;

        private EditorCoroutine(IEnumerator routine, Action onFinish)
        {
            OnFinish = onFinish;
            mRoutine = routine;
        }

        public Action OnFinish { get; }

        public static EditorCoroutine Start(IEnumerator routine, Action onFinish = null)
        {
            var coroutine = new EditorCoroutine(routine, onFinish);
            coroutine.Start();
            return coroutine;
        }

        private void Start()
        {
            //Debug.Log("start");
            EditorApplication.update += Update;
        }

        public void Stop()
        {
            //Debug.Log("stop");
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            /* NOTE: no need to try/catch MoveNext,
             * if an IEnumerator throws its next iteration returns false.
             * Also, Unity probably catches when calling EditorApplication.update.
             */

            //Debug.Log("update");
            if (!mRoutine.MoveNext())
            {
                Stop();
                OnFinish?.Invoke();
            }
        }
    }
}
#endif