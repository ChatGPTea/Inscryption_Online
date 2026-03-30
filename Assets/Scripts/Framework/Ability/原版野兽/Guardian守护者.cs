using System.Collections.Generic;
using DG.Tweening;
using Inscryption;
using QFramework;
using UnityEngine;
using UnityEngine.Rendering;

public class Guardian守护者 : Effect
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
        if (this.GetModel<ISlotsModel>().CurrentSlot >= 0)
        {
            mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(this.GetModel<ISlotsModel>().CurrentSlot, !isMyCard)));
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
        foreach (var target in mTarget)
        {
            if (target?.GetComponent<Slot>() != null)
            {
                this.SendCommand(new RemoveCardInSlotCommand(mSelf.transform.parent.parent.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.SetParent(target.GetComponent<Slot>().slotVisual.transform);
                this.SendCommand(new AddCardInSlotCommand(mSelf.transform.gameObject,
                    this.GetComponentInParent<Slot>().slotIndex
                    , isMyCard));
                mSelf.transform.DOMove(this.GetComponentInParent<Slot>().slotVisual.position, 0.5f)
                    .SetEase(Ease.InSine);
                mSelf.transform.DORotateQuaternion(this.GetComponentInParent<Slot>().slotVisual.rotation, 0.5f)
                    .SetEase(Ease.InSine);
            }
        }
    }

    //亡语效果
    public override void OnDieExecute()
    {
    }
}