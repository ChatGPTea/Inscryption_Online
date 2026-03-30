using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class RemoveCardToEnemyCardListCommand : AbstractCommand
    {
        private readonly GameObject mCardInHand;

        private ICardSystem mCardSystem;

        public RemoveCardToEnemyCardListCommand(GameObject cardInHand)
        {
            mCardInHand = cardInHand;
        }

        protected override void OnExecute()
        {
            mCardSystem = this.GetSystem<ICardSystem>();
            mCardSystem.mEnemyCardInHands.Remove(mCardInHand);
        }
    }
}