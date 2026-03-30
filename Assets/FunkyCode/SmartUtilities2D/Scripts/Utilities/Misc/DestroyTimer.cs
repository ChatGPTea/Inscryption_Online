using UnityEngine;

namespace FunkyCode.Utilities
{
    public class DestroyTimer : MonoBehaviour
    {
        private TimerHelper timer;

        private void Start()
        {
            timer = TimerHelper.Create();
        }

        private void Update()
        {
            if (timer.GetMillisecs() > 2000) Destroy(gameObject);
        }
    }
}