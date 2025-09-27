using SmartHome.Class;

namespace SmartHome.Controllers.Abstract
{
    public abstract class ControllerBase
    {
        private readonly string _id;
        protected IReadOnlyList<Room> Rooms { get; }

        public string Id => _id;

        protected ControllerBase(IEnumerable<Room> rooms)
        {
            _id = Guid.NewGuid().ToString();
            Rooms = rooms?.ToList() ?? throw new ArgumentNullException(nameof(rooms));
        }

        public abstract void Execute();
    }
}