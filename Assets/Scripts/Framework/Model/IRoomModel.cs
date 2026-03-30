using QFramework;

namespace Inscryption
{
    public interface IRoomModel : IModel
    {
        BindableProperty<int> SlotCount { get; }
        BindableProperty<int> HealthCount { get; }
        BindableProperty<int> CurrentHealth { get; }
    }

    public class RoomModel : AbstractModel, IRoomModel
    {
        public BindableProperty<int> SlotCount { get; } = new()
        {
            Value = 5
        };

        public BindableProperty<int> HealthCount { get; } = new()
        {
            Value = 7
        };

        public BindableProperty<int> CurrentHealth { get; } = new()
        {
            Value = 0
        };

        protected override void OnInit()
        {
            
        }
    }
}