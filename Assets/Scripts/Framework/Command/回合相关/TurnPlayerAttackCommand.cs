using QFramework;

namespace Inscryption
{
    public class TurnPlayerAttackCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            var mTurnSystem = this.GetSystem<ITurnSystem>();
            mTurnSystem.State.Value = TurnState.PlayerAttack;
        }
    }
}