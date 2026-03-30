using QFramework;
using UnityEngine;

public class EnableMono : MonoBehaviour
{
    /// <summary>
    ///     用于给普通ui加音效
    /// </summary>
    public bool isFirstLoad = true;

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
}