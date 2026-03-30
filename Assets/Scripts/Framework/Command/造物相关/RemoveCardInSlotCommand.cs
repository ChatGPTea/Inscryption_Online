using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class RemoveCardInSlotCommand : AbstractCommand
    {
        private readonly GameObject mSlot;
        private readonly int mCardInSlotIndex;
        private readonly bool isPlayerCard;

        private ISlotsModel mSlotsModel;

        public RemoveCardInSlotCommand(GameObject Slot, int cardInSlotIndex, bool isPlayerCard)
        {
            mSlot = Slot;
            mCardInSlotIndex = cardInSlotIndex;
            this.isPlayerCard = isPlayerCard;
        }
        
        protected override void OnExecute()
        {
            mSlotsModel = this.GetModel<ISlotsModel>();
            if (isPlayerCard)
            {
                mSlotsModel.PlayerCardInSlotDic[mCardInSlotIndex] = mSlot;
            }
            else
            {
                mSlotsModel.EnemyCardInSlotDic[mCardInSlotIndex] = mSlot;
            }
            mSlotsModel.UpdateBuff();
        }
    }
}