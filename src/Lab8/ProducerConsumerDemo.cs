using System.Collections.Concurrent;
using LogSaveService;

namespace Lab8
{
    internal static class ProducerConsumerDemo
    {
        public static async Task Main()
        {
            SimpleLogger.Info("Запущен {ProducerConsumerDemo}:");
            var queue = new BlockingCollection<int>();
            var cts = new CancellationTokenSource();

            var consumerTasks = new Task[3];
            for (var i = 0; i < 3; i++)
            {
                var id = i + 1;
                consumerTasks[i] = Task.Run(() => ConsumeItems(queue, id, cts.Token), cts.Token);
            }

            var producerTask = Task.Run(() => ProduceItems(queue, cts.Token), cts.Token);
            await producerTask;
            queue.CompleteAdding();
            await Task.WhenAll(consumerTasks);

            SimpleLogger.Info("Все задачи завершены.");
        }

        private static void ProduceItems(BlockingCollection<int> queue, CancellationToken ct)
        {
            var rand = new Random();
            var count = rand.Next(50, 100); // 50–100

            for (var i = 1; i <= count; i++)
            {
                if (ct.IsCancellationRequested) break;
                SimpleLogger.Info($"Producer добавил: {i}");
                queue.Add(i, ct);
                Thread.Sleep(10);
            }
        }

        private static async Task ConsumeItems(BlockingCollection<int> queue, int id, CancellationToken ct)
        {
            foreach (var item in queue.GetConsumingEnumerable(ct))
            {
                SimpleLogger.Info($"Consumer {id} обрабатывает: {item}");
                await Task.Delay(100, ct); // имитация обработки
                var result = item * 2;
                SimpleLogger.Info($"Consumer {id} завершил: {item} = {result}");
            }
            SimpleLogger.Info($"Consumer {id} завершён.");
        }
    }
}