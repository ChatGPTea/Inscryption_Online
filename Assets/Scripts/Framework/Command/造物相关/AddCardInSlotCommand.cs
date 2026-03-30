using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class AddCardInSlotCommand : AbstractCommand
    {
        private readonly GameObject mCardInSlot;
        private readonly int mCardInSlotIndex;
        private readonly bool isPlayerCard;

        private ISlotsModel mSlotsModel;

        public AddCardInSlotCommand(GameObject cardInSlot, int cardInSlotIndex, bool isPlayerCard)
        {
            mCardInSlot = cardInSlot;
            mCardInSlotIndex = cardInSlotIndex;
            this.isPlayerCard = isPlayerCard;
        }
        
        protected override void OnExecute()
        {
            mSlotsModel = this.GetModel<ISlotsModel>();
            if (isPlayerCard)
            {
                mSlotsModel.PlayerCardInSlotDic[mCardInSlotIndex] = mCardInSlot;
            }
            else
            {
                mSlotsModel.EnemyCardInSlotDic[mCardInSlotIndex] = mCardInSlot;
            }
            mSlotsModel.UpdateBuff();
        }
    }
}