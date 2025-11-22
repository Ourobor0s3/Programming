using System.Diagnostics;
using LogSaveService;

namespace Lab9_Asynchronous
{
    internal static class Program
    {
        private static async Task Main()
        {
            SimpleLogger.Info("=== ЗАДАНИЕ 1 ===");
            await RunTask1();

            SimpleLogger.Info("=== ЗАДАНИЕ 2 ===");
            await RunTask2();
        }

        private static async Task RunTask1()
        {
            var cts = new CancellationTokenSource();

            Task.Run(() =>
                {
                    SimpleLogger.Info("Нажмите 'c' для отмены...");
                    if (Console.ReadKey(true).Key == ConsoleKey.C)
                        cts.Cancel();
                });

            try
            {
                SimpleLogger.Info("Последовательный запуск...");
                var sw = Stopwatch.StartNew();

                try
                {
                    var r1 = await ProgramService.GetDataAsync("A", cts.Token);
                    var r2 = await ProgramService.GetDataAsync("B", cts.Token);
                    var r3 = await ProgramService.GetDataAsync("C", cts.Token);
                    SimpleLogger.Info($"РЕЗУЛЬТАТЫ: {r1}, {r2}, {r3}");
                }
                catch (OperationCanceledException)
                {
                    SimpleLogger.Error("Последовательный запуск отменён.");
                }

                sw.Stop();
                SimpleLogger.Info($"Время последовательного: {sw.ElapsedMilliseconds} ms");

                SimpleLogger.Info("Параллельный запуск...");
                cts = new CancellationTokenSource();
                sw.Restart();

                var tasks = new[]
                {
                    ProgramService.GetDataAsync("A", cts.Token),
                    ProgramService.GetDataAsync("B", cts.Token),
                    ProgramService.GetDataAsync("C", cts.Token),
                };

                try
                {
                    var results = await Task.WhenAll(tasks);
                    SimpleLogger.Info("РЕЗУЛЬТАТЫ: " + string.Join(", ", results));
                }
                catch (OperationCanceledException)
                {
                    SimpleLogger.Error("Параллельный запуск отменён.");
                }
                catch (Exception ex)
                {
                    SimpleLogger.Error($"Ошибка в параллельных задачах: {ex.Message}");
                }

                sw.Stop();
                SimpleLogger.Info($"Время параллельного: {sw.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                SimpleLogger.Error("Глобальная ошибка: " + ex.Message);
            }
        }

        private static async Task RunTask2()
        {
            var ids = Enumerable.Range(1, 20).ToList();
            SimpleLogger.Info($"Всего ids = {ids.Count}");

            var results = new List<int>();
            var errors = new List<(int id, Exception ex)>();

            var sw = Stopwatch.StartNew();

            var cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                SimpleLogger.Info("Нажмите 'c' для отмены загрузок...");
                if (Console.ReadKey(true).Key == ConsoleKey.C)
                    cts.Cancel();
            });

            const int maxDegree = 4;
            var semaphore = new SemaphoreSlim(maxDegree);

            var tasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync(cts.Token);

                try
                {
                    var res = await ProgramService.LoadItemAsync(id, cts.Token);
                    lock (results) results.Add(res);
                }
                catch (OperationCanceledException)
                {
                    lock (errors) errors.Add((id, new OperationCanceledException("Отменено пользователем")));
                }
                catch (Exception ex)
                {
                    SimpleLogger.Error($"[{id}] {ex.Message}");
                    lock (errors) errors.Add((id, ex));
                }
                finally
                {
                    semaphore.Release();
                }
            });

            try
            {
                await Task.WhenAll(tasks);
            }
            catch
            {
                // ignored
            }

            sw.Stop();

            SimpleLogger.Info("=== ИТОГИ ===");
            SimpleLogger.Info($"Успешно: {results.Count}");
            SimpleLogger.Error($"Ошибок: {errors.Count}");
            foreach (var e in errors)
                SimpleLogger.Error($"  id={e.id}: {e.ex.Message}");

            SimpleLogger.Info($"Было ли прерывание: {cts.IsCancellationRequested}");
            SimpleLogger.Info($"Время выполнения: {sw.ElapsedMilliseconds} ms");
        }
    }
}