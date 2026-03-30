using System;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LightCycleBuffer
    {
        public Gradient gradient = new();
    }

    [Serializable]
    public class LightDayProperties
    {
        [Range(0, 360)] public float shadowOffset;

        public AnimationCurve shadowHeight = new();

        public AnimationCurve shadowAlpha = new();
    }

    [ExecuteInEditMode]
    public class LightCycle : MonoBehaviour
    {
        [Range(0, 1)] public float time;

        public LightDayProperties dayProperties = new();

        public LightCycleBuffer[] nightProperties = new LightCycleBuffer[1]; // lightmap

        private void LateUpdate()
        {
            var lightmapPresets = Lighting2D.Profile.lightmapPresets;

            if (lightmapPresets == null) return;

            /*
            if (Input.GetMouseButton(0)&& Input.touchCount > 1) { //
                time += Time.deltaTime * 0.05f;

                time = time % 1;
            }*/

            var time360 = time * 360;

            // Day Lighting Properties
            var height = dayProperties.shadowHeight.Evaluate(time);
            var alpha = dayProperties.shadowAlpha.Evaluate(time);

            if (height < 0.01f) height = 0.01f;

            if (alpha < 0) alpha = 0;

            Lighting2D.DayLightingSettings.height = height;
            Lighting2D.DayLightingSettings.ShadowColor.a = alpha;
            Lighting2D.DayLightingSettings.direction = time360 + dayProperties.shadowOffset;

            // Dynamic Properties
            for (var i = 0; i < nightProperties.Length; i++)
            {
                if (i >= lightmapPresets.list.Length) return;

                var buffer = nightProperties[i];

                if (buffer == null) continue;

                var color = buffer.gradient.Evaluate(time);

                var lightmapPreset = lightmapPresets.list[i];
                lightmapPreset.darknessColor = color;
            }
        }

        public void SetTime(float setTime)
        {
            time = setTime;
        }
    }
}