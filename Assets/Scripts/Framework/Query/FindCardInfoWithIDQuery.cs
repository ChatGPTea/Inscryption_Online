using System.Linq;
using QFramework;

namespace Inscryption
{
    public class FindCardInfoWithIDQuery : AbstractQuery<CardInfo>
    {
        private readonly int mCardID;

        public FindCardInfoWithIDQuery(int cardID)
        {
            mCardID = cardID;
        }

        protected override CardInfo OnDo()
        {
            var mCardLibModel = this.GetModel<ICardLibModel>();
            return mCardLibModel.CardLib.FirstOrDefault(card => card.CardID == mCardID);
        }
    }
}