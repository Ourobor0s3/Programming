using System;

namespace SmartHome
{
    public class Thermostat(string name) : Device(name), IControllable
    {
        private double _temperature = 20.0;

        public double Temperature
        {
            get => _temperature;
            set
            {
                if (value is < -20.0 or > 50.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Temperature must be between -20°C and 50°C.");
                _temperature = value;
            }
        }

        protected override string Type => "Thermostat";

        public override string Status()
        {
            return $"{base.Status()} Temperature: {Temperature:F1}°C";
        }
    }
}