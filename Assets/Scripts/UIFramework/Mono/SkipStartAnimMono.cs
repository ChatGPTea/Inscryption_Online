using UnityEngine;

public class SkipStartAnimMono : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
            SkipTimelineToEnd();
    }

    private void SkipTimelineToEnd()
    {
        WndManager.Instance.GetWnd<StartWnd>().ExitAnim();
    }
}