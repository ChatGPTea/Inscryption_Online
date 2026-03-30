using System.Collections.Generic;
using Inscryption;
using UnityEngine;

[CreateAssetMenu(fileName = "CountInfo", menuName = "CountInfo")]
public class CountInfo : ScriptableObject
{
    public List<Sprite> mCount_blood = new();
    public List<Sprite> mCount_bone = new();
    public List<Sprite> mCountCost = new();
    public List<Sprite> mPixel_blood = new();
    public List<Sprite> mPixel_bone = new();

    public Sprite FindSpriteWithCostAndCostType(int cost, CostType costType)
    {
        switch (costType)
        {
            case CostType.bone:
                if (cost == 0)
                {
                    return null;
                }
                else if (cost <= 10)
                {
                    return mCount_bone[cost - 1];
                }

                return mCount_bone[9];

            case CostType.sacrifice:
                if (cost == 0)
                {
                    return null;
                }
                else if (cost <= 4)
                {
                    return mCount_blood[cost - 1];
                }

                return mCount_blood[3];

            default:
                Debug.LogWarning($"未知的 CostType: {costType}");
                return null;
        }
    }
    
    public Sprite FindPixelSpriteWithCostAndCostType(int cost, CostType costType)
    {
        switch (costType)
        {
            case CostType.bone:
                if (cost == 0)
                {
                    return null;
                }
                else if (cost <= 13)
                {
                    return mPixel_bone[cost - 1];
                }

                return mPixel_bone[12];

            case CostType.sacrifice:
                if (cost == 0)
                {
                    return null;
                }
                else if (cost <= 4)
                {
                    return mPixel_blood[cost - 1];
                }

                return mPixel_blood[3];

            default:
                Debug.LogWarning($"未知的 CostType: {costType}");
                return null;
        }
    }
}