using System.Collections.Generic;
using DG.Tweening;
using Inscryption;
using QFramework;
using UnityEngine;

public class BaseAttack : Effect
{
    public int mValue = -1;
    public List<GameObject> mTarget = new List<GameObject>();
    public CardInSlot mSelf = null;
    public int mSelfIndex = -1;
    public bool isMyCard = false;

    // 当前攻击目标的索引
    private int currentTargetIndex = 0;

    public override void Init()
    {
        mSelf = gameObject.GetComponent<CardInSlot>();
        mSelfIndex = mSelf.GetComponentInParent<Slot>().slotIndex;

        mValue = mSelf.mData.Attack;
        isMyCard = mSelf.isMycard;

        if (mSelf.transform?.GetComponent<BifurcatedStrike兵分两路>() != null)
        {
            mTarget.Clear();
            if (mSelfIndex > 0)
            {
                mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex - 1, isMyCard)));
            }

            if (mSelfIndex < this.GetModel<IRoomModel>().SlotCount.Value - 1)
            {
                mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex + 1, isMyCard)));
            }
        }
        else if (mSelf.transform?.GetComponent<TrifurcatedStrike兵分三路>() != null)
        {
            mTarget.Clear();
            if (mSelfIndex > 0)
            {
                mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex - 1, isMyCard)));
            }

            mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex, isMyCard)));

            if (mSelfIndex < this.GetModel<IRoomModel>().SlotCount.Value - 1)
            {
                mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex + 1, isMyCard)));
            }
        }
        else
        {
            mTarget.Clear();
            mTarget.Add(this.SendQuery(new FindTargetByIndexQuery(mSelfIndex, isMyCard)));
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

    //攻击效果 - 开始攻击第一个目标
    public override void OnAttackExecute()
    {
        currentTargetIndex = 0;
        AttackNextTarget();
    }

    // 攻击下一个目标
    private void AttackNextTarget()
    {
        // 检查是否还有目标
        if (currentTargetIndex >= mTarget.Count)
        {
            return; // 所有目标都已攻击完毕
        }

        GameObject currentTarget = mTarget[currentTargetIndex];
        if (currentTarget == null)
        {
            // 当前目标为空，跳过
            currentTargetIndex++;
            AttackNextTarget();
            return;
        }

        // 先检查自己是否有 Slot 组件，如果没有再查父级
        Slot slot = currentTarget.GetComponent<Slot>() ?? currentTarget.GetComponentInParent<Slot>();

        if (slot != null)
        {
            this.GetModel<ISlotsModel>().OnEnemyTryAttack(isMyCard, slot.slotIndex);
        }

        // 执行攻击动画
        AttackTarget(currentTarget, () =>
        {
            // 攻击完成回调 - 目标受到伤害
            DealDamageToTarget(currentTarget);

            // 移动到下一个目标
            currentTargetIndex++;

            // 等待一小段时间再攻击下一个目标（可选，让动画更流畅）
            DOVirtual.DelayedCall(0.1f, () => { AttackNextTarget(); });
        });
    }

    public void AttackTarget(GameObject currentTarget, System.Action onComplete)
    {
        // 随机播放 cardattack_player 或 cardattack_card
        string soundName = "cardattack_" + (Random.Range(0, 2) == 0 ? "player" : "card");
        AudioKit.PlaySound("resources://Sound/Cards/" + soundName);
        WndManager.Instance.GetWnd<GameWnd>().mSlotsGroup.SwapChildrenIndex(isMyCard);
        mSelf.transform.DOKill();

        // 记录父级基准
        Vector3 parentPos = mSelf.transform.parent ? mSelf.transform.parent.position : mSelf.transform.position;
        Quaternion parentRot = mSelf.transform.parent ? mSelf.transform.parent.rotation : mSelf.transform.rotation;

        Sequence seq = DOTween.Sequence();

        // 1. 蓄力后退 + 转正
        seq.Append(mSelf.transform.DORotateQuaternion(parentRot, 0.3f));

        // 2. 撞击目标
        Vector3 hitPos = isMyCard
            ? new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y - 1f,
                currentTarget.transform.position.z)
            : new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1f,
                currentTarget.transform.position.z);
        seq.Append(mSelf.transform.DOMove(hitPos, 0.8f).SetEase(Ease.InOutElastic));

        // 3. 返回原位
        seq.Append(mSelf.transform.DOMove(parentPos, 0.2f).SetEase(Ease.OutBack))
            .Join(mSelf.transform.DORotateQuaternion(parentRot, 0.2f))
            .OnComplete(() =>
            {
                // 动画序列完成，调用回调
                onComplete?.Invoke();
            });
    }

    // 单独处理伤害逻辑
    private void DealDamageToTarget(GameObject currentTarget)
    {
        if (currentTarget == null)
        {
            return;
        }

        Vector3 hitPos = isMyCard
            ? new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y - 1f,
                currentTarget.transform.position.z)
            : new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y + 1f,
                currentTarget.transform.position.z);

        // 攻击卡牌目标
        if (currentTarget.GetComponentInChildren<CardInSlot>() != null &&
            currentTarget.GetComponentInChildren<Waterborne水袭>() == null)
        {
            currentTarget.transform.DOPunchScale(Vector2.one * 0.2f, 0.2f);
            var targetCard = currentTarget.GetComponentInChildren<CardInSlot>();

            // 高跳效果
            if (currentTarget.GetComponentInChildren<Reach高跳>() != null)
            {
                if (mSelf.transform?.GetComponentInChildren<TouchOfDeath死神之触>() != null)
                {
                    targetCard.GetDamage(1120, mSelf.gameObject);
                }
                else
                {
                    targetCard.GetDamage(mValue, mSelf.gameObject);
                }

                return;
            }

            // 空袭效果
            if (mSelf.transform?.GetComponentInChildren<Airborne空袭>() != null)
            {
                if (isMyCard)
                {
                    WndManager.Instance.GetWnd<GameWnd>().mGameMono.GetDamage(hitPos, mValue);
                }
                else
                {
                    WndManager.Instance.GetWnd<GameWnd>().mGameMono.GetDamage(hitPos, -mValue);
                }

                return;
            }

            // 死神之触效果
            if (mSelf.transform?.GetComponentInChildren<TouchOfDeath死神之触>() != null)
            {
                targetCard.GetDamage(1120, mSelf.gameObject);
            }
            else
            {
                targetCard.GetDamage(mValue, mSelf.gameObject);
            }
        }
        // 攻击空位或水袭效果
        else if (currentTarget.GetComponent<Slot>() != null ||
                 currentTarget.GetComponentInChildren<Waterborne水袭>() != null)
        {
            if (isMyCard)
            {
                WndManager.Instance.GetWnd<GameWnd>().mGameMono.GetDamage(hitPos, mValue);
            }
            else
            {
                WndManager.Instance.GetWnd<GameWnd>().mGameMono.GetDamage(hitPos, -mValue);
            }
        }
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