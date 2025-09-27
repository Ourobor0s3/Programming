using Lab3.Controllers.Abstract;
using Lab3.Interface;

namespace Lab3.Class
{
    public class Room
    {
        private readonly List<Device> _devices;
        public string Name { get; }

        public Room(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Room name cannot be null or empty.", nameof(name));
            _devices = [];
            Name = name;
        }

        public void AddDevice(Device device)
        {
            ArgumentNullException.ThrowIfNull(device);
            _devices.Add(device);
        }

        public bool RemoveDevice(string id)
        {
            return _devices.RemoveAll(d => d.Id == id) > 0;
        }

        public IReadOnlyList<Device> GetDevices() => _devices.AsReadOnly();

        public void TurnAllOn()
        {
            foreach (var device in _devices)
            {
                if (device is IControllable controllable)
                {
                    controllable.TurnOn();
                }
            }
        }
    }
}