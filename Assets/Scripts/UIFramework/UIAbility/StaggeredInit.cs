using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using QFramework;

public class StaggeredInit : MonoBehaviour
{
    [SerializeField] private float initDelay = 0.1f; // 每个子物体之间的延迟

    private void Start()
    {
        WndManager.Instance?.GetWnd<GameWnd>().mGameBeginEvent.Register(() =>
        {
            Init();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    public void Init()
    {
        StartCoroutine(StaggeredInitializeChildren());
    }
    
    private IEnumerator StaggeredInitializeChildren()
    {
        // 获取所有子物体
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            
            // 检查子物体是否有 InitMove 组件
            InitMove initMove = child.GetComponent<InitMove>();
            if (initMove != null)
            {
                // 调用 Init 方法，传递延迟参数
                initMove.Init(); // 转换为毫秒
                
                // 等待指定的延迟时间
                yield return new WaitForSeconds(initDelay);
            }
        }
    }
}