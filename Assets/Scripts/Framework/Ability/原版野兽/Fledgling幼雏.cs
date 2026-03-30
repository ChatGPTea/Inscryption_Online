using Inscryption;
using UnityEngine;

public class Fledgling幼雏 : Effect
{
    public int Count = 0;

    //战吼效果
    public override void Init()
    {
    }

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
        Count++;
        if (Count == 1)
        {
            var mSelf = gameObject.GetComponent<CardInSlot>();
            var mCardID = mSelf.mCurrentCardID;
            if (mCardID == 1014)
            {
                mSelf.PlayFlipAnimation(1013);
            }
            else if (mCardID == 1030)
            {
                mSelf.PlayFlipAnimation(1029);
            }
            else if (mCardID == 1039)
            {
                mSelf.PlayFlipAnimation(1010);
            }
        }
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