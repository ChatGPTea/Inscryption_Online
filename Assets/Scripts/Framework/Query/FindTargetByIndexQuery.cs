using System.Linq;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    public class FindTargetByIndexQuery : AbstractQuery<GameObject>
    {
        private readonly int mIndex;
        private readonly bool mIsMyCard;

        public FindTargetByIndexQuery(int index, bool isMyCard)
        { 
            mIndex = index;
            mIsMyCard = isMyCard;
        }

        protected override GameObject OnDo()
        {
            var mSlotsModel = this.GetModel<ISlotsModel>();
            var mTargetList = mIsMyCard ? mSlotsModel.EnemyCardInSlotDic : mSlotsModel.PlayerCardInSlotDic;
            
            // 边界检查：确保索引在有效范围内
            int slotCount = this.GetModel<IRoomModel>().SlotCount.Value;
            if (mIndex < 0 || mIndex >= slotCount)
            {
                return null;  // 索引无效，返回null
            }
            
            // 检查目标槽位是否有卡片
            if (!mTargetList.ContainsKey(mIndex) || mTargetList[mIndex] == null)
            {
                return null;  // 槽位为空，返回null
            }
            
            return mTargetList[mIndex];
        }
    }
}