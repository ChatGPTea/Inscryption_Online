using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class RemoveCardToPlayerCardListCommand : AbstractCommand
    {
        private readonly GameObject mCardInHand;

        private ICardSystem mCardSystem;

        public RemoveCardToPlayerCardListCommand(GameObject cardInHand)
        {
            mCardInHand = cardInHand;
        }

        protected override void OnExecute()
        {
            mCardSystem = this.GetSystem<ICardSystem>();
            mCardSystem.mPlayerCardInHands.Remove(mCardInHand);
        }
    }
}