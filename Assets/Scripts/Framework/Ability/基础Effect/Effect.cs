using Inscryption;
using UnityEngine;

public abstract class Effect : InscryptionController
{
    public abstract void Init();
    public abstract void OnPlaceExecute();
    public abstract void BeforeAttackExecute();
    public abstract void OnAttackExecute();
    public abstract void AfterAttackExecute();
    public abstract void BeforeTakeAttackExecute();
    public abstract void OnTakeAttackExecute();
    public abstract void AfterTakeAttackExecute();
    public abstract void OnPlayerTurnBeginExecute();
    public abstract void OnPlayerTurnEndExecute();
    public abstract void OnEnemyPlaceExecute();
    public abstract void OnDieExecute();
}