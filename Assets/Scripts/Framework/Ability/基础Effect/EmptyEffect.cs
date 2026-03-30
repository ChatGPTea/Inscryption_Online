using System.Collections.Generic;
using Inscryption;

public class EmptyEffect : Effect
{
    public int mValue = -1;
    public List<CardInSlot> mTarget = new List<CardInSlot>();
    public CardInSlot mSelf = null;
    public bool isMyCard = false;
    
    public override void Init()
    {
        
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
        
    }

    //亡语效果
    public override void OnDieExecute()
    {
        
    }
}