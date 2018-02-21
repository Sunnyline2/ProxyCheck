using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckProxy.Core.Job
{
    public class JobManager
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public bool IsRunning { get; private set; }
 
        public async Task ExecuteAsync<T>(JobAsync<T> job) where T : class, new()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                IsRunning = true;
                await job.ExecuteAsync();
            }
            finally
            {
                IsRunning = false;
                _semaphoreSlim.Release();
            }
        }

        public void Execute<T>(Job<T> job) where T : class, new()
        {
            _semaphoreSlim.Wait();
            try
            {
                IsRunning = true;
                job.Execute();
            }
            finally
            {
                IsRunning = false;
                _semaphoreSlim.Release();
            }
        }
    }
}
