using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CommandButtonMono : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    // 添加选中状态标志
    private bool isSelected = false;

    public bool IsSelected
    {
        get => isSelected;
        set => isSelected = value;
    }

    private void OnEnable()
    {
        if (isFirstLoad) return;

        var _randomNum = Random.Range(0, 5);
        AudioKit.PlaySound("resources://Sound/Command/" + _randomNum);
    }

    private void OnDisable()
    {
        isFirstLoad = false;
    }

    #region 设置颜色部分

    // 所有子物体中的 Image 和 TMP_Text 组件, 以及原始颜色
    private Image[] childImages;
    private TMP_Text[] childTexts;
    private readonly Dictionary<Image, Color> originalImageColors = new();
    private readonly Dictionary<TMP_Text, Color> originalTextColors = new();
    private bool isFirstLoad = true;
    private bool isInteracting;

    private void Start()
    {
        // 遍历所有子节点，保存原始颜色
        childImages = GetComponentsInChildren<Image>(true);
        childTexts = GetComponentsInChildren<TMP_Text>(true);

        foreach (var img in childImages)
            if (img != null && img.gameObject != gameObject)
                if (!originalImageColors.ContainsKey(img))
                    originalImageColors[img] = img.color;

        foreach (var txt in childTexts)
            if (txt != null && txt.gameObject != gameObject)
                if (!originalTextColors.ContainsKey(txt))
                    originalTextColors[txt] = txt.color;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        var _randomNum = Random.Range(0, 5);
        AudioKit.PlaySound("resources://Sound/Command/" + _randomNum);

        SetInteractionState(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        var _randomNum = Random.Range(0, 5);
        AudioKit.PlaySound("resources://Sound/Command/" + _randomNum);

        SetInteractionState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetInteractionState(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetInteractionState(false);
    }

    /// <summary>
    ///     设置交互状态，并相应地修改所有子对象的颜色
    /// </summary>
    private void SetInteractionState(bool isInteractingNow)
    {
        // 如果按钮已被选中，不修改颜色
        if (isSelected) return;

        isInteracting = isInteractingNow;

        if (isInteracting)
            SetAllChildrenToBlack();
        else
            RestoreAllChildrenOriginalColors();
    }

    /// <summary>
    ///     将所有子 Image 和 TMP_Text 设置为黑色
    /// </summary>
    private void SetAllChildrenToBlack()
    {
        foreach (var img in childImages)
            if (img != null && originalImageColors.ContainsKey(img))
                img.color = Color.black;

        foreach (var txt in childTexts)
            if (txt != null && originalTextColors.ContainsKey(txt))
                txt.color = Color.black;
    }

    /// <summary>
    ///     从字典中恢复所有子对象的颜色为原始值
    /// </summary>
    private void RestoreAllChildrenOriginalColors()
    {
        foreach (var pair in originalImageColors)
            if (pair.Key != null)
                pair.Key.color = pair.Value;

        foreach (var pair in originalTextColors)
            if (pair.Key != null)
                pair.Key.color = pair.Value;
    }

    #endregion
}