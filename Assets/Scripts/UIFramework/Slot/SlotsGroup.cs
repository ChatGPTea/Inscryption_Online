using System.Collections.Generic;
using Inscryption;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class SlotsGroup : InscryptionController
{
   /// <summary>
   ///     获取相对应的组件
   /// </summary>
   public Transform mPlayerSlotsGroup;

    public Transform mEnemySlotsGroup;
    public Dictionary<int,GameObject> mEnemySlots = new();
    public Dictionary<int,GameObject> mPlayerSlots = new();

    /// <summary>
    ///     预制体
    /// </summary>
    private GameObject mSlotPrefab;

    /// <summary>
    ///     变量信息
    /// </summary>
    private int mSlotsCount;

    /// <summary>
    ///     Model
    /// </summary>
    private ISlotsModel mSlotsModel;


    public void Start()
    {
        //获取组件
        mPlayerSlotsGroup = transform.Find("PlayerSlotGroup").transform;
        mEnemySlotsGroup = transform.Find("EnemySlotGroup").transform;
        mSlotPrefab = Resources.Load<GameObject>("Prefabs/Slot");


        //本地玩家读取本地房间的信息
        //TODO Client也需要同步
        mSlotsCount = this.SendQuery(new LocalSlotsCountQuery());

        //矫正父节点宽度
        AdjustSlotsGroupWidth(mPlayerSlotsGroup, mSlotsCount);
        AdjustSlotsGroupWidth(mEnemySlotsGroup, mSlotsCount);

        //生成卡槽
        GenerateSlots(mPlayerSlotsGroup, true);
        GenerateSlots(mEnemySlotsGroup, false);
        this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () =>
        {
            mPlayerSlotsGroup.GetComponent<HorizontalLayoutGroup>().enabled = false;
            mEnemySlotsGroup.GetComponent<HorizontalLayoutGroup>().enabled = false;
        });
    }

    #region 初始化卡槽

    private void GenerateSlots(Transform slotsGroup, bool isPlayerSlots)
    {
        for (var i = 0; i < mSlotsCount; i++)
        {
            var Slot = Instantiate(mSlotPrefab, slotsGroup);
            if (isPlayerSlots)
                mPlayerSlots.Add(i,Slot);
            else
                mEnemySlots.Add(i,Slot);
            this.SendCommand(new AddSlotInitCommand(Slot,i,isPlayerSlots));
        }

        if (isPlayerSlots)
            foreach (var index in mPlayerSlots)
            {
                var slot = mPlayerSlots[index.Key].GetComponent<Slot>();
                slot.isMySlot = true;
                slot.TotalSlots = mSlotsCount;
                slot.maxRotationAngle = -slot.maxRotationAngle;
                slot.maxVerticalOffset = -slot.maxVerticalOffset;
                slot.Init();
            }
        else
            foreach (var index in mEnemySlots)
            {
                var slot = mEnemySlots[index.Key].GetComponent<Slot>();
                slot.isMySlot = false;
                slot.TotalSlots = mSlotsCount;
                slot.Init();
            }
    }

    /// <summary>
    ///     根据卡槽数量动态调整 UI 容器
    /// </summary>
    private void AdjustSlotsGroupWidth(Transform slotsGroup, int slotCount)
    {
        var rectTransform = slotsGroup.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError($"AdjustSlotsGroupWidth: {slotsGroup.name} 缺少 RectTransform 组件！", slotsGroup);
            return;
        }

        // 限制槽数量的合理范围，避免插值越界
        slotCount = Mathf.Clamp(slotCount, 1, 8);
        var minWidth = 400f;
        var maxWidth = 1200f;
        var minSlots = 1f;
        var maxSlots = 8f;

        // 插值公式：width = minWidth + (slotCount - minSlots) / (maxSlots - minSlots) * (maxWidth - minWidth)
        var t = (slotCount - minSlots) / (maxSlots - minSlots);
        var targetWidth = Mathf.Lerp(minWidth, maxWidth, t);

        var newSize = rectTransform.sizeDelta;
        newSize.x = targetWidth;
        rectTransform.sizeDelta = newSize;
    }

    #endregion

    #region 交换卡槽遮盖顺序

    public void SwapChildrenIndex(bool isMyTurn)
    {
        int indexA = mPlayerSlotsGroup.GetSiblingIndex();
        int indexB = mEnemySlotsGroup.GetSiblingIndex();
    
        if (isMyTurn)
        {
            // 我的回合：确保玩家槽位在敌人槽位之前（更小的SiblingIndex）
            if (indexA < indexB)
            {
                // 玩家槽位索引大于敌人，需要交换
                SwapTwoChildren(mPlayerSlotsGroup, mEnemySlotsGroup);
            }
        }
        else
        {
            // 敌方回合：确保敌人槽位在玩家槽位之前
            if (indexB < indexA)
            {
                // 敌人槽位索引大于玩家，需要交换
                SwapTwoChildren(mPlayerSlotsGroup, mEnemySlotsGroup);
            }
        }
    }
    
    private void SwapTwoChildren(Transform childA, Transform childB)
    {
        if (childA == null || childB == null)
        {
            Debug.LogError("子物体引用为空");
            return;
        }
    
        if (childA.parent != childB.parent)
        {
            Debug.LogError("两个子物体不在同一个父物体下");
            return;
        }
    
        int indexA = childA.GetSiblingIndex();
        int indexB = childB.GetSiblingIndex();
    
        childA.SetSiblingIndex(indexB);
        childB.SetSiblingIndex(indexA);
    }

    #endregion

}