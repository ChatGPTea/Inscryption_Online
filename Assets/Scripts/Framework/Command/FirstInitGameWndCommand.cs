using QFramework;

namespace Inscryption
{
    public class FirstInitGameWndCommand : AbstractCommand
    {
        private ICardSystem mCardSystem;
        protected override void OnExecute()
        {
            mCardSystem = this.GetSystem<ICardSystem>();
            mCardSystem.ReDrawCardCount();
        }
    }
}