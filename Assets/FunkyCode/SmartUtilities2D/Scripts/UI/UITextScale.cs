using UnityEngine;
using UnityEngine.UI;

namespace FunkyCode.Utilities
{
    [ExecuteInEditMode]
    public class UITextScale : MonoBehaviour
    {
        public float ratio = 10f;
        public Rect rect = new(0, 0, 100, 100);
        private RectTransform rectTransform;

        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
            rectTransform = GetComponent<RectTransform>();
            Update();
        }

        private void Update()
        {
            text.fontSize = (int)(Screen.height * (ratio / 100f));
            rectTransform.anchorMin = rect.min / 100;
            rectTransform.anchorMax = rect.max / 100;
        }
    }
}