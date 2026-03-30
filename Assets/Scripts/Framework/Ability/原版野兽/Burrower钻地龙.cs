using System.Collections.Generic;
using DG.Tweening;
using Inscryption;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;

public class Burrower钻地龙 : Effect
{
    public List<GameObject> mTarget = new List<GameObject>();
    public CardInSlot mSelf = null;
    public int mSelfIndex = -1;
    public bool isMyCard = false;

    public override void Init()
    {
        mSelf = gameObject.GetComponent<CardInSlot>();
        isMyCard = mSelf.isMycard;

        mTarget.Clear();
        if (this.GetModel<ISlotsModel>().CurrentSlot == -1) return;
        mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(this.GetModel<ISlotsModel>().CurrentSlot,!isMyCard)));
    }

    //战吼效果
    public override void OnPlaceExecute()
    {
    }

    //攻击前效果
    public override void BeforeAttackExecute()
    {
    }

    //攻击效果
    public override void OnAttackExecute()
    {

    }

    //攻击后效果
    public override void AfterAttackExecute()
    {
    }

    //承伤前效果
    public override void BeforeTakeAttackExecute()
    {
        foreach (var target in mTarget)
        {
            if (target?.GetComponent<Slot>() != null)
            {
                RectTransform cardRect = target?.GetComponent<Slot>().transform.GetComponent<RectTransform>();
                // 记录当前层级位置（在父级中的索引）
                int originalSiblingIndex = cardRect.GetSiblingIndex();
                this.SendCommand(new RemoveCardInSlotCommand(mSelf.transform.parent.parent.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.SetParent(target.GetComponent<Slot>().slotVisual.transform);
                this.SendCommand(new AddCardInSlotCommand(mSelf.transform.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.DOMove(this.GetComponentInParent<Slot>().slotVisual.position,0.5f).SetEase(Ease.InSine);
                mSelf.transform.DORotateQuaternion(this.GetComponentInParent<Slot>().slotVisual.rotation, 0.5f)
                    .SetEase(Ease.InSine).OnComplete(() =>
                    {
                        cardRect.SetSiblingIndex(originalSiblingIndex);

                    });
            }
        }
    }

    //承伤效果
    public override void OnTakeAttackExecute()
    {
    }

    //承伤后效果
    public override void AfterTakeAttackExecute()
    {
    }

    //玩家回合开始效果
    public override void OnPlayerTurnBeginExecute()
    {
    }

    //玩家回合结束效果
    public override void OnPlayerTurnEndExecute()
    {
    }

    //敌方放置卡牌后效果
    public override void OnEnemyPlaceExecute()
    {

    }

    //亡语效果
    public override void OnDieExecute()
    {
    }
}
