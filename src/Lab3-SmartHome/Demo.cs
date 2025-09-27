using System;
using System.Collections.Generic;
using Lab3.Class;
using Lab3.Controllers;

namespace Lab3
{
    public static class Demo
    {
        private static List<Room> _rooms = [];
        public static void Run1()
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
                        DemoService.AddRoom(_rooms);
                        break;
                    case "2":
                        DemoService.AddDeviceToRoom(_rooms);
                        break;
                    case "3":
                        DemoService.TurnAllOnInRoom(_rooms);
                        break;
                    case "4":
                        DemoService.ShowAllStatuses(_rooms);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        public static void Run2()
        {
            // Создаём две комнаты
            var room1 = new Room("Living Room");
            var room2 = new Room("Bedroom");

            // В Living Room: термостат (18°C) и выключенный обогреватель
            var thermostat = new Thermostat("Living Thermostat") { Temperature = 18.0 };
            var heater = new Heater("Main Heater") { TargetTemperature = 22.0 };
            // heater изначально выключен (IsOn = false по умолчанию)

            room1.AddDevice(thermostat);
            room1.AddDevice(heater);

            // В Bedroom: просто лампа (для контраста)
            room2.AddDevice(new Light("Bed Light") { Brightness = 50 });

            // Создаём контроллер
            var homeController = new HomeController([room1, room2]);

            // Показываем начальное состояние
            Console.WriteLine("=== Initial Status ===");
            DemoService.ShowAllStatuses([room1, room2]);

            // Запускаем контроллер
            Console.WriteLine("\n=== Running HomeController ===");
            homeController.Run();

            // Показываем финальное состояние
            Console.WriteLine("=== Final Status ===");
            DemoService.ShowAllStatuses([room1, room2]);
        }
    }
}