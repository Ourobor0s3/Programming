using LogSaveService;

namespace Lab9_Asynchronous
{
    public static class ProgramService
    {
        public static async Task<string> GetDataAsync(string name, CancellationToken token)
        {
            SimpleLogger.Info($"[{name}] start");
            await Task.Delay(1000, token);
            SimpleLogger.Info($"[{name}] finish");
            return $"Data for {name}";
        }

        public static async Task<int> LoadItemAsync(int id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            SimpleLogger.Info($"[{id}] start");

            var rnd = new Random();
            await Task.Delay(rnd.Next(100, 1000), token);

            if (rnd.NextDouble() < 0.15)
                throw new InvalidOperationException("Random failure");

            SimpleLogger.Info($"[{id}] finish");
            return id * 10;
        }
    }
}