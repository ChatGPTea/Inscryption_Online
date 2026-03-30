using System.Collections.Generic;
using Inscryption;
using QFramework;
using UnityEngine;

public class DamBuilder筑坝师 : Effect
{
    public int mValue = -1;
    public List<GameObject> mTarget = new List<GameObject>();
    public CardInSlot mSelf = null;
    public int mSelfIndex = -1;
    public bool isMyCard = false;

    public override void Init()
    {
        mSelf = gameObject.GetComponent<CardInSlot>();
        mSelfIndex = mSelf.GetComponentInParent<Slot>().slotIndex;

        mValue = mSelf.mData.Attack;
        isMyCard = mSelf.isMycard;

        mTarget.Clear();
        if (mSelfIndex > 0)
        {
            mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex - 1, !isMyCard)));
        }
        
        if (mSelfIndex < this.GetModel<IRoomModel>().SlotCount.Value - 1)
        {
            mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex + 1, !isMyCard)));
        }
    }

    //战吼效果
    public override void OnPlaceExecute()
    {
        foreach (var target in mTarget)
        {
            if (target?.GetComponent<Slot>() != null)
            {
                target.GetComponent<Slot>().SummonCardInSlot(101,isMyCard);
            }
        }
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