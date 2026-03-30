using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public class LightFlicker : MonoBehaviour
    {
        public float flickersPerSecond = 15f;
        public float flickerRangeMin = -0.1f;
        public float flickerRangeMax = 0.1f;
        private float lightAlpha;

        private Light2D lightSource;
        private TimerHelper timer;

        private void Start()
        {
            lightSource = GetComponent<Light2D>();
            lightAlpha = lightSource.color.a;

            timer = TimerHelper.Create();
        }

        private void Update()
        {
            if (timer == null)
            {
                timer = TimerHelper.Create();
                return;
            }

            if (timer.GetMillisecs() > 1000f / flickersPerSecond)
            {
                var tempAlpha = lightAlpha;
                tempAlpha = tempAlpha + Random.Range(flickerRangeMin, flickerRangeMax);
                lightSource.color.a = tempAlpha;
                timer.Reset();
            }
        }
    }
}