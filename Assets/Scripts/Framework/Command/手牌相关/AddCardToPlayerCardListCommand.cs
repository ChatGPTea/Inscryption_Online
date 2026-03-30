using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class AddCardToPlayerCardListCommand : AbstractCommand
    {
        private readonly GameObject mCardInHand;
        private readonly bool mIsMainCard;

        private ICardSystem mCardSystem;

        public AddCardToPlayerCardListCommand(GameObject cardInHand, bool isMainCard)
        {
            mCardInHand = cardInHand;
            mIsMainCard = isMainCard;
        }

        protected override void OnExecute()
        {
            mCardSystem = this.GetSystem<ICardSystem>();
            mCardSystem.mPlayerCardInHands.Add(mCardInHand);
            if (mIsMainCard)
            {
                mCardSystem.mPlayerCardCount.Value--;
            }
        }
    }
}