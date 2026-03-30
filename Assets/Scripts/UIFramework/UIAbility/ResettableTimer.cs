using UnityEngine;

public class ResettableTimer : MonoBehaviour
{
    [Header("计时设置")]
    public float disableDelay = 2f; // 默认2秒后关闭
    
    private bool isTimerRunning = false;
    private float timer = 0f;

    // 当物体被激活时调用 - 总是重新开始计时
    private void OnEnable()
    {
        ResetAndStartTimer();
    }

    // 当物体被禁用时调用
    private void OnDisable()
    {
        StopTimer();
    }

    // 更新函数中处理计时
    private void Update()
    {
        if (!isTimerRunning) return;
        
        timer += Time.deltaTime;
        
        if (timer >= disableDelay)
        {
            DisableSelf();
        }
    }

    // 重置并重新开始计时
    public void ResetAndStartTimer()
    {
        isTimerRunning = true;
        timer = 0f;
    }
    
    // 停止计时
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    // 关闭自身
    private void DisableSelf()
    {
        isTimerRunning = false;
        gameObject.SetActive(false);
    }
}