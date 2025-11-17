using LogSaveService;

namespace Lab8
{
    internal static class RaceConditionDemo
    {
        private static int _counter;
        private static object _lock = new object();

        public static void Run1()
        {
            SimpleLogger.Info("Запущен {RaceConditionDemo}:");
            Console.Write("Введите количество потоков: ");
            var threadCountStr = Console.ReadLine();
            var threadCount = int.Parse(string.IsNullOrEmpty(threadCountStr) ?  "2" : threadCountStr);

            Console.Write("Введите число, до которого нужно дойти инкрементом: ");
            var maxCountStr = Console.ReadLine();
            var maxCount = int.Parse(string.IsNullOrEmpty(maxCountStr) ?  "10" : maxCountStr);

            Console.Write("Выводить шаги? (y/n): ");
            var showSteps = Console.ReadLine()?.ToLower() == "y";

            var results = new[]
            {
                RunUnsafeIncrement(threadCount, maxCount, showSteps),
                RunLockIncrement(threadCount, maxCount, showSteps),
                RunInterlockedIncrement(threadCount, maxCount, showSteps)
            };

            SimpleLogger.Info("Результаты:");
            SimpleLogger.Info("Метод\t\t\tВремя (мс)\tИтоговое значение");
            foreach (var res in results)
            {
                SimpleLogger.Info($"{res.Name,-20}\t{res.ElapsedMs,8}\t\t{res.CounterValue}");
            }
            SimpleLogger.Info($"Число потоков: {threadCount}, инкремент: {maxCount}");
        }

        private static (string Name, long ElapsedMs, int CounterValue) RunUnsafeIncrement(int threadCount, int maxCount, bool showSteps)
        {
            _counter = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var tasks = new Task[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (_counter < maxCount)
                    {
                        if (showSteps) SimpleLogger.Info($"Unsafe: {_counter}");
                        _counter++;
                    }
                });
            }
            Task.WaitAll(tasks);
            sw.Stop();
            return ("UnsafeIncrement", sw.ElapsedMilliseconds, _counter);
        }

        private static (string Name, long ElapsedMs, int CounterValue) RunLockIncrement(
            int threadCount,
            int maxCount,
            bool showSteps)
        {
            _counter = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var tasks = new Task[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (_counter < maxCount)
                    {
                        lock (_lock)
                        {
                            if (_counter >= maxCount) return;
                            if (showSteps) SimpleLogger.Info($"Lock: {_counter}");
                            _counter++;
                        }
                    }
                });
            }
            Task.WaitAll(tasks);
            sw.Stop();
            return ("LockIncrement", sw.ElapsedMilliseconds, _counter);
        }

        private static (string Name, long ElapsedMs, int CounterValue) RunInterlockedIncrement(
            int threadCount,
            int maxCount,
            bool showSteps)
        {
            _counter = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var tasks = new Task[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    while (_counter < maxCount)
                    {
                        var val = Interlocked.Increment(ref _counter);
                        if (showSteps) SimpleLogger.Info($"Interlocked: {val}");
                        if (val >= maxCount) break;
                    }
                });
            }
            Task.WaitAll(tasks);
            sw.Stop();
            return ("InterlockedIncrement", sw.ElapsedMilliseconds, _counter);
        }
    }
}