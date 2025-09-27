using SmartHome.Class;

namespace SmartHome
{
    public static class DemoService
    {
        public static void AddRoom(List<Room> rooms)
        {
            Console.Write("Enter room name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Room name cannot be empty.");
                return;
            }

            rooms.Add(new Room(name));
            Console.WriteLine($"Room '{name}' added successfully.");
        }

        public static void AddDeviceToRoom(List<Room> rooms)
        {
            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available. Add a room first.");
                return;
            }

            Console.WriteLine("Available rooms:");
            for (var i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name}");
            }

            Console.Write("Select room number: ");
            if (!int.TryParse(Console.ReadLine(), out int roomIndex) || roomIndex < 1 || roomIndex > rooms.Count)
            {
                Console.WriteLine("Invalid room number.");
                return;
            }

            Room selectedRoom = rooms[roomIndex - 1];

            Console.WriteLine("Select device type:");
            Console.WriteLine("1. Light");
            Console.WriteLine("2. Thermostat");
            Console.Write("Choice: ");
            var deviceType = Console.ReadLine();

            Console.Write("Enter device name: ");
            var deviceName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(deviceName))
            {
                Console.WriteLine("Device name cannot be empty.");
                return;
            }

            try
            {
                switch (deviceType)
                {
                    case "1":
                        var light = new Light(deviceName);
                        Console.Write("Enter brightness (0-100): ");
                        if (int.TryParse(Console.ReadLine(), out int brightness))
                            light.Brightness = brightness;
                        selectedRoom.AddDevice(light);
                        Console.WriteLine($"Light '{deviceName}' added to room '{selectedRoom.Name}'.");
                        break;
                    case "2":
                        var thermostat = new Thermostat(deviceName);
                        Console.Write("Enter temperature (-20 to 50): ");
                        if (double.TryParse(Console.ReadLine(), out double temperature))
                            thermostat.Temperature = temperature;
                        selectedRoom.AddDevice(thermostat);
                        Console.WriteLine($"Thermostat '{deviceName}' added to room '{selectedRoom.Name}'.");
                        break;
                    default:
                        Console.WriteLine("Invalid device type.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void TurnAllOnInRoom(List<Room> rooms)
        {
            if (rooms.Count == 0)
            {
                Console.WriteLine("No rooms available.");
                return;
            }

            Console.WriteLine("Available rooms:");
            for (int i = 0; i < rooms.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {rooms[i].Name}");
            }

            Console.Write("Select room number to turn all devices ON: ");
            if (!int.TryParse(Console.ReadLine(), out var roomIndex) || roomIndex < 1 || roomIndex > rooms.Count)
            {
                Console.WriteLine("Invalid room number.");
                return;
            }

            rooms[roomIndex - 1].TurnAllOn();
            Console.WriteLine("All controllable devices turned ON.");
        }

        public static void ShowAllStatuses(IEnumerable<Room> rooms)
        {
            foreach (var room in rooms)
            {
                Console.WriteLine($"\n--- Room: {room.Name} ---");
                var devices = room.GetDevices();
                if (devices.Count == 0)
                {
                    Console.WriteLine("  No devices.");
                }
                else
                {
                    foreach (var device in devices)
                    {
                        Console.WriteLine($"{device.Status()}");
                    }
                }
            }
        }
    }
}