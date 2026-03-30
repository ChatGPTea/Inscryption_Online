using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class FadeInEffect : MonoBehaviour
{
    public CanvasGroup mCanvasGroup;

    void Start()
    {
        mCanvasGroup.alpha = 0;
        if (WndManager.Instance.GetWnd<GameWnd>() == null) return;
        WndManager.Instance.GetWnd<GameWnd>().mGameBeginEvent.Register(() =>
        {
            Init();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    public void Init()
    {
        mCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutBack);
    }
    
}