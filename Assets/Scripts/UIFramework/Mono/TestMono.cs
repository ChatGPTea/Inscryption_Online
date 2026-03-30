using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class TestMono : InscryptionController
    {
        public void Attack()
        {
            this.GetSystem<ITurnSystem>().State.Value = TurnState.PlayerAttack;
        }
        
        public void EnemySelect(int index)
        {
            this.SendCommand(new EnemyCardSelectCommand(index));
        }

        public struct EnemySlotSelectEvent
        {
            public int slotIndex;
        }

        public void EnemySummon(int index)
        {
            TypeEventSystem.Global.Send(new EnemySlotSelectEvent()
            {
                slotIndex = index,
            });
        }

        public struct SummonCardInSlotEvent
        {
            public int cardID;
            public int slotIndex;
            public bool isMyCard;
        }

        public void SummonCardInSlot(int cardID, int slotIndex, bool isMyCard)
        {
            TypeEventSystem.Global.Send(new SummonCardInSlotEvent()
            {
                cardID = cardID,
                slotIndex = slotIndex,
                isMyCard = isMyCard,
            });
        }
    }
}