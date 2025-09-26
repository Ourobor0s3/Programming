namespace SmartHome
{
    public class Thermostat : Device, IControllable
    {
        public override string Type { get; }
        public string _temperature { get; protected set; }

        public Thermostat(string name) : base(name)
        {
            Type = "Thermostat";
        }
    }
}