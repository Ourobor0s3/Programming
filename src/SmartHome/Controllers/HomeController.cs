using SmartHome.Class;
using SmartHome.Controllers.Abstract;
using SmartHome.Interface;

namespace SmartHome.Controllers
{
    public class HomeController(IEnumerable<Room> rooms) : ControllerBase(rooms)
    {
        private const decimal TemperatureThreshold = 20.0m;

        public override void Execute()
        {
            Console.WriteLine("=== HomeController: Checking sensors and heaters ===");

            foreach (var room in Rooms)
            {
                // Собираем сенсоры и обогреватели в комнате
                List<ISensor> sensors = [];
                List<Heater> heaters = [];

                foreach (var device in room.GetDevices())
                {
                    if (device is ISensor sensor)
                        sensors.Add(sensor);
                    if (device is Heater heater)
                        heaters.Add(heater);
                }

                // Для каждого сенсора проверяем температуру
                foreach (var sensor in sensors)
                {
                    if (sensor.TryRead(out var currentTemp))
                    {
                        Console.WriteLine($"Room '{room.Name}': Sensor reads {currentTemp}°C");

                        if (currentTemp < TemperatureThreshold)
                        {
                            Console.WriteLine($"  → Temperature below threshold ({TemperatureThreshold}°C). Turning ON heaters...");

                            foreach (var heater in heaters.Where(heater => !heater.IsOn))
                            {
                                heater.TurnOn();
                            }
                        }
                        else
                            Console.WriteLine($"  → Temperature OK.");
                    }
                    else
                        Console.WriteLine($"Room '{room.Name}': Failed to read sensor.");
                }
            }
        }

        public void Run()
        {
            try
            {
                Execute();
                Console.WriteLine("HomeController execution completed.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HomeController error: {ex.Message}");
            }
        }
    }
}