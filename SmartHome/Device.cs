namespace SmartHome
{
    public abstract class Device(string name)
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private string Name
        {
            get => name;
            init
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be null or empty.", nameof(value));
                name = value;
            }
        }

        private bool IsOn { get; set; } = false;

        protected abstract string Type { get; }

        public virtual void TurnOn()
        {
            IsOn = true;
            SimpleLogger.Info($"Device {Id}:{Name} turned on");
        }

        public virtual void TurnOff()
        {
            IsOn = false;
            SimpleLogger.Info($"Device {Id}:{Name} turned off");

        }

        public virtual string Status()
        {
            return $"Device {Id}:{Name} status - {IsOn}, type - {Type}";
        }
    }
}