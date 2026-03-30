using UnityEngine;

namespace FunkyCode.Utilities
{
    [ExecuteInEditMode]
    public class UICanvasScale : MonoBehaviour
    {
        public bool screenRatioScale;
        public Rect rect = new(0, 0, 100, 100);

        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            Update();
        }

        private void Update()
        {
            if (screenRatioScale)
            {
                var tempRect = rect;

                tempRect.height *= (float)Screen.width / Screen.height;
                tempRect.y -= tempRect.height / 4;

                rectTransform.anchorMin = tempRect.min / 100;
                rectTransform.anchorMax = tempRect.max / 100;
            }
            else
            {
                rectTransform.anchorMin = rect.min / 100;
                rectTransform.anchorMax = rect.max / 100;
            }
        }
    }
}