using System;
using DG.Tweening;
using QFramework;
using UnityEngine;

public class InitMove : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(
            GetComponent<RectTransform>().localPosition.x + 50f, GetComponent<RectTransform>().localPosition.y - 200f,
            0);
    }


    public void Init()
    {
        gameObject.SetActive(true);
        GetComponent<RectTransform>()
            .DOAnchorPos(
                new Vector3(GetComponent<RectTransform>().localPosition.x - 50f,
                    GetComponent<RectTransform>().localPosition.y + 200f, 0), 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                string myName = gameObject.name;

                // 检查名字是否不是 "card1" 且不是 "card2"
                if (myName != "Card_1" && myName != "Card_2" && myName != "Card_3")
                {
                    gameObject.SetActive(false);
                }
            });
    }
}