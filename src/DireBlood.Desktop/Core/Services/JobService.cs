using System.Threading;
using System.Threading.Tasks;

namespace DireBlood.Core.Services
{
    public class JobService : IJobService
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public bool IsRunning { get; private set; }

        public async Task ExecuteAsync<T>(JobAsync<T> job) where T : class, new()
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                IsRunning = true;
                await job.ExecuteAsync();
            }
            finally
            {
                IsRunning = false;
                semaphoreSlim.Release();
            }
        }

        public void Execute<T>(Job<T> job) where T : class, new()
        {
            semaphoreSlim.Wait();
            try
            {
                IsRunning = true;
                job.Execute();
            }
            finally
            {
                IsRunning = false;
                semaphoreSlim.Release();
            }
        }
    }
}