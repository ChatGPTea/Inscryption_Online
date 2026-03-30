using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class EnemyCardSelectCommand : AbstractCommand
    {
        private readonly int index;
        
        public EnemyCardSelectCommand(int index)
        {
            this.index = index;
        }
        
        protected override void OnExecute()
        {
            var cardSystem = this.GetSystem<ICardSystem>();
            for (var i = 0; i < cardSystem.mEnemyCardInHands.Count; i++)
            {
                if (cardSystem.mEnemyCardInHands[i].GetComponent<CardInHand>().CardIndex == index)
                {
                    var card = cardSystem.mEnemyCardInHands[i].GetComponent<CardInHand>();
                    if (card.mCurrentState == CardInHandState.OnAnim) return;

                    if (card.mCurrentState == CardInHandState.Idle)
                    {
                        card.mCurrentState = CardInHandState.Selected;
                        card.OnSelected();
                    }
                    else if (card.mCurrentState == CardInHandState.Selected)
                    {
                        card.mCurrentState = CardInHandState.Idle;
                        card.UnSelected();
                    }
                }
            }
        }
    }
}