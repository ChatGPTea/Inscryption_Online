using QFramework;

namespace Inscryption
{
    public class LocalSlotsCountQuery : AbstractQuery<int>
    {
        protected override int OnDo()
        {
            var mRoomModel = this.GetModel<IRoomModel>();
            return mRoomModel.SlotCount.Value;
        }
    }
}