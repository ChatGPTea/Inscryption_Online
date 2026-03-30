using UnityEngine;

public class PositionRestorer : MonoBehaviour
{
    [Header("位置恢复设置")]
    [Tooltip("是否启用位置恢复功能")]
    public bool enablePositionRestore = true;
    
    [Tooltip("是否在每帧检查位置变化")]
    public bool checkEveryFrame = true;
    
    [Tooltip("检查间隔时间（秒），仅在checkEveryFrame为false时有效")]
    public float checkInterval = 0.1f;
    
    // 私有变量
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 lastCheckedPosition;
    private float lastCheckTime;
    
    void Start()
    {
        // 记录初始位置和旋转
        RecordInitialTransform();
        
        if (!checkEveryFrame)
        {
            lastCheckTime = Time.time;
        }
    }
    
    void Update()
    {
        if (!enablePositionRestore) return;
        
        if (checkEveryFrame)
        {
            CheckAndRestorePosition();
        }
        else
        {
            // 按时间间隔检查
            if (Time.time - lastCheckTime >= checkInterval)
            {
                CheckAndRestorePosition();
                lastCheckTime = Time.time;
            }
        }
    }
    
    /// <summary>
    /// 记录当前的变换信息作为初始状态
    /// </summary>
    public void RecordInitialTransform()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        lastCheckedPosition = initialPosition;
    }
    
    /// <summary>
    /// 检查位置是否发生变化，如果变化则恢复
    /// </summary>
    private void CheckAndRestorePosition()
    {
        Vector3 currentPosition = transform.position;
        
        // 检查位置是否发生变化（使用阈值避免浮点数精度问题）
        if (Vector3.Distance(currentPosition, lastCheckedPosition) > 0.001f)
        {
            RestoreToInitialPosition();
        }
        
        lastCheckedPosition = currentPosition;
    }
    
    /// <summary>
    /// 恢复到初始位置
    /// </summary>
    public void RestoreToInitialPosition()
    {
        if (!enablePositionRestore) return;
        var caret = transform.parent.Find("Caret");
        caret.transform.position = transform.position;
        caret.transform.rotation = transform.rotation;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }
    
}