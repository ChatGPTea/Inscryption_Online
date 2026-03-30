using System;
using QFramework;
using TMPro;

namespace Inscryption
{
    public class RoomListWndMono : InscryptionController
    {
        private IRoomModel mRoomModel;
        private TMP_Text mSlotCountTip;
        private TMP_Text mHPCountTip;

        private void Start()
        {
            mSlotCountTip = transform.Find("SlotSetting/Tip_1").GetComponent<TMP_Text>();
            mHPCountTip = transform.Find("HPSetting/Tip_1").GetComponent<TMP_Text>();
            
            mRoomModel = this.GetModel<IRoomModel>();
            ReDrawRoomList();
            mRoomModel.SlotCount.Register(newValue =>
            {
                ReDrawRoomList();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
            mRoomModel.HealthCount.Register(newValue =>
            {
                ReDrawRoomList();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void ReDrawRoomList()
        {
            mSlotCountTip.text = mRoomModel.SlotCount.Value + "槽";
            mHPCountTip.text = mRoomModel.HealthCount.Value + "血";
        }

        #region 调整房间设置

        public void AddSlotCount()
        {   
            if (mRoomModel.SlotCount.Value == 6)
                return;
            mRoomModel.SlotCount.Value++;
        }

        public void RemoveSlotCount()
        {
            if (mRoomModel.SlotCount.Value == 4)
                return;
            mRoomModel.SlotCount.Value--;
        }

        public void AddHealthCount()
        {
            if (mRoomModel.HealthCount.Value == 20)
                return;
            mRoomModel.HealthCount.Value++;
        }

        public void RemoveHealthCount()
        {
            if (mRoomModel.HealthCount.Value == 5)
                return;
            mRoomModel.HealthCount.Value--;
        }

        #endregion
    }
}