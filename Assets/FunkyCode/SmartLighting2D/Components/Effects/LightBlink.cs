using UnityEngine;

namespace FunkyCode
{
    public class LightBlink : MonoBehaviour
    {
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.black;

        private Light2D lightingSource;

        private void Start()
        {
            lightingSource = GetComponent<Light2D>();
        }


        private void Update()
        {
            var time = Time.realtimeSinceStartup;
            var step = Mathf.Cos(time);
            var color = Color.Lerp(primaryColor, secondaryColor, Mathf.Abs(step));

            lightingSource.color = color;

            lightingSource.meshMode.alpha = color.a * 0.5f;
        }
    }
}