namespace SmartHome
{
    public class Heater(string name) : Device(name), IControllable
    {
        public double TargetTemperature { get; set; } = 22.0; // по умолчанию

        public override string Type => "Heater";

        public override string Status()
        {
            return $"{base.Status()} Target: {TargetTemperature:F1}°C";
        }
    }
}