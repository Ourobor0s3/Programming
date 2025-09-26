namespace SmartHome
{
    public abstract class Device
    {
        public string _id { get; }
        public string _name { get; init; }
        public bool _isOn { get; protected set; }

        public abstract string Type { get; }

        protected Device(string name)
        {
            _id = Guid.NewGuid().ToString();
            _name = name;
        }

        public virtual void TurnOn()
        {
            _isOn = true;
            SimpleLogger.Info($"Device {_id}:{_name} turned on");
        }

        public virtual void TurnOff()
        {
            _isOn = false;
            SimpleLogger.Info($"Device {_id}:{_name} turned off");

        }

        public virtual string Status()
        {
            return $"Device {_id}:{_name} status - {_isOn}, type - {Type}";
        }
    }
}