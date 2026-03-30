using System.Collections.Generic;
using DG.Tweening;
using Inscryption;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;

public class LooseTail断尾求生 : Effect
{
    public List<GameObject> mTarget = new List<GameObject>();
    public CardInSlot mSelf = null;
    public int mSelfIndex = -1;
    public bool isMyCard = false;
    public bool isRight = true;
    public bool OnluOnce = false;
    public override void Init()
    {
        mSelf = gameObject.GetComponent<CardInSlot>();
        mSelfIndex = mSelf.GetComponentInParent<Slot>().slotIndex;
        isMyCard = mSelf.isMycard;
        mTarget.Clear();
        
        int slotCount = this.GetModel<IRoomModel>().SlotCount.Value;
        
        if (mSelfIndex == 0)
        {
            // 最左边，只能向右
            var target = this.SendQuery(new FindTargetByIndexQuery(mSelfIndex + 1, !isMyCard));
            if (target != null)
            {
                mTarget.Add(target);
            }
            isRight = true;
            mSelf.ChangeDirection(isRight, Abilities.Sprinter冲刺能手);
        }
        else if (mSelfIndex == slotCount - 1)
        {
            // 最右边，只能向左
            var target = this.SendQuery(new FindTargetByIndexQuery(mSelfIndex - 1, !isMyCard));
            if (target != null)
            {
                mTarget.Add(target);
            }
            isRight = false;
            mSelf.ChangeDirection(isRight, Abilities.Sprinter冲刺能手);
        }
        else
        {
            // 中间位置，根据当前方向选择目标
            if (isRight)
            {
                var target = this.SendQuery(new FindTargetByIndexQuery(mSelfIndex + 1, !isMyCard));
                if (target != null)
                {
                    mTarget.Add(target);
                }
            }
            else
            {
                var target = this.SendQuery(new FindTargetByIndexQuery(mSelfIndex - 1, !isMyCard));
                if (target != null)
                {
                    mTarget.Add(target);
                }
            }
        }

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
        if (OnluOnce) return;
        OnluOnce = true;
        foreach (var target in mTarget)
        {
            if (target?.GetComponent<Slot>() != null && target?.GetComponent<CardInSlot>() == null)
            {
                RectTransform cardRect = target?.GetComponent<Slot>().transform.GetComponent<RectTransform>();
                // 记录当前层级位置（在父级中的索引）
                int originalSiblingIndex = cardRect.GetSiblingIndex();
                mSelf.GetComponentInParent<Slot>().SummonCardInSlot(108, isMyCard);
                this.SendCommand(new RemoveCardInSlotCommand(mSelf.transform.parent.parent.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.SetParent(target.GetComponent<Slot>().slotVisual.transform);
                this.SendCommand(new AddCardInSlotCommand(mSelf.transform.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.DOMove(this.GetComponentInParent<Slot>().slotVisual.position,0.5f).SetEase(Ease.InSine);
                mSelf.transform.DORotateQuaternion(this.GetComponentInParent<Slot>().slotVisual.rotation, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    mSelf.PlayFlipAnimation(107,true);
                    cardRect.SetSiblingIndex(originalSiblingIndex);
                });
            }
            else
            {
                isRight = !isRight;
                mSelf.ChangeDirection(isRight,Abilities.LooseTail断尾求生);
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