using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class AddCardToEnemyListCommand : AbstractCommand
    {
        private readonly GameObject mCardInHand;

        private ICardSystem mCardSystem;

        public AddCardToEnemyListCommand(GameObject cardInHand)
        {
            mCardInHand = cardInHand;
        }

        protected override void OnExecute()
        {
            mCardSystem = this.GetSystem<ICardSystem>();
            mCardSystem.mEnemyCardInHands.Add(mCardInHand);
        }
    }
}