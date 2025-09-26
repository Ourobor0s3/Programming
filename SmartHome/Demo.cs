namespace SmartHome
{
    public static class Demo
    {
        private static List<Room> _rooms = [];
        public static void Run()
        {
            while (true)
            {
                Console.WriteLine("\n=== Smart Home Manager ===");
                Console.WriteLine("1. Add Room");
                Console.WriteLine("2. Add Device to Room");
                Console.WriteLine("3. Turn All Devices ON in a Room");
                Console.WriteLine("4. Show All Devices Status (Grouped by Room)");
                Console.WriteLine("5. Exit");
                Console.Write("Choose an option: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddRoom();
                        break;
                    case "2":
                        AddDeviceToRoom();
                        break;
                    case "3":
                        TurnAllOnInRoom();
                        break;
                    case "4":
                        ShowAllStatuses();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        private static void AddRoom()
    {
        Console.Write("Enter room name: ");
        var name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Room name cannot be empty.");
            return;
        }

        _rooms.Add(new Room(name));
        Console.WriteLine($"Room '{name}' added successfully.");
    }

    private static void AddDeviceToRoom()
    {
        if (_rooms.Count == 0)
        {
            Console.WriteLine("No rooms available. Add a room first.");
            return;
        }

        Console.WriteLine("Available rooms:");
        for (var i = 0; i < _rooms.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_rooms[i].Name}");
        }

        Console.Write("Select room number: ");
        if (!int.TryParse(Console.ReadLine(), out int roomIndex) || roomIndex < 1 || roomIndex > _rooms.Count)
        {
            Console.WriteLine("Invalid room number.");
            return;
        }

        Room selectedRoom = _rooms[roomIndex - 1];

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

    private static void TurnAllOnInRoom()
    {
        if (_rooms.Count == 0)
        {
            Console.WriteLine("No rooms available.");
            return;
        }

        Console.WriteLine("Available rooms:");
        for (int i = 0; i < _rooms.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_rooms[i].Name}");
        }

        Console.Write("Select room number to turn all devices ON: ");
        if (!int.TryParse(Console.ReadLine(), out int roomIndex) || roomIndex < 1 || roomIndex > _rooms.Count)
        {
            Console.WriteLine("Invalid room number.");
            return;
        }

        _rooms[roomIndex - 1].TurnAllOn();
        Console.WriteLine("All controllable devices turned ON.");
    }

    private static void ShowAllStatuses()
    {
        if (_rooms.Count == 0)
        {
            Console.WriteLine("No rooms available.");
            return;
        }

        foreach (var room in _rooms)
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
                    Console.WriteLine($"  {device.Status()}");
                }
            }
        }
    }
    }
}