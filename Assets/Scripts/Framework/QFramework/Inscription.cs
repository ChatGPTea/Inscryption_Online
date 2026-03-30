using QFramework;

namespace Inscryption
{
    public class Inscription : Architecture<Inscription>
    {
        protected override void Init()
        {
            RegisterModel<IRoomModel>(new RoomModel());
            RegisterModel<ISlotsModel>(new SlotModel());
            RegisterModel<ICardLibModel>(new CardListModel());
            
            RegisterSystem<ITimeSystem>(new TimeSystem());
            RegisterSystem<ICardSystem>(new CardSystem());
            RegisterSystem<ITurnSystem>(new TurnSystem());
        }
    }
}