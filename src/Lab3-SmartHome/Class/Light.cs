using Lab3.Controllers.Abstract;
using Lab3.Interface;

namespace Lab3.Class
{
    public class Light(string name) : Device(name), IControllable
    {
        private int _brightness = 0;

        public int Brightness
        {
            get => _brightness;
            set
            {
                if (value is < 0 or > 100)
                    throw new ArgumentOutOfRangeException(nameof(value), "Brightness must be between 0 and 100.");
                _brightness = value;
            }
        }

        public override string Type => "Light";

        public override string Status()
        {
            return $"{base.Status()} Brightness: {Brightness}%";
        }
    }
}