namespace SmartHome
{
    public class Light : Device, IControllable
    {
        public string _brightness { get; protected set; }
        public override string Type { get; }

        public Light(string name) : base(name)
        {
            Type = "Light";
        }
    }
}