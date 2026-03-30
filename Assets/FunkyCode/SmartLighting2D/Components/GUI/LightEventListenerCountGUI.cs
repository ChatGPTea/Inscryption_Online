using UnityEngine;

namespace FunkyCode
{
    [ExecuteInEditMode]
    public class LightEventListenerCountGUI : MonoBehaviour
    {
        private static Texture pointTexture;

        private LightEventListenerCount lightEventReceiver;

        private void OnEnable()
        {
            lightEventReceiver = GetComponent<LightEventListenerCount>();
        }

        private void OnGUI()
        {
            if (Camera.main == null) return;

            Vector2 middlePoint = Camera.main.WorldToScreenPoint(transform.position);

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;

            var display = lightEventReceiver.lights.Count.ToString();

            var size = Screen.height / 20;

            var style = new GUIStyle();
            style.fontSize = size;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;

            var pointSize = Screen.height / 80;

            GUI.Label(new Rect(middlePoint.x - 50, Screen.height - middlePoint.y - 50, 100, 100), display, style);
        }
    }
}