using QFramework;

namespace Inscryption
{
    public class EnemySummonFormHandCommand : AbstractCommand
    {
        private readonly Slot mSlot;

        public EnemySummonFormHandCommand(Slot mSlot)
        {
            this.mSlot = mSlot;
        }
        
        protected override void OnExecute()
        {
            var cardSystem = this.GetSystem<ICardSystem>();
            var slotModel = this.GetModel<SlotModel>();
            for (var i = 0; i < cardSystem.mEnemyCardInHands.Count; i++)
            {
                if (cardSystem.mEnemyCardInHands[i].GetComponent<CardInHand>().mCurrentState ==
                    CardInHandState.Selected)
                {
                    var card = cardSystem.mEnemyCardInHands[i].GetComponent<CardInHand>();
                    card.EnemySummonCardFormHand(mSlot);
                    break;
                }
            }
        }
    }
}