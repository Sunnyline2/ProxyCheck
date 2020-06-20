using System.Threading;
using System.Threading.Tasks;

namespace DireBlood.Core.Jobs.Services
{
    public class JobManagementService
    {
        private readonly SemaphoreSlim semaphoreSlim;
        public JobManagementService(JobManagementConfiguration configuration)
        {
            Guard.Against.Null(configuration, nameof(configuration));

            semaphoreSlim = new SemaphoreSlim(0, configuration.MaxThreadsCount);
        }

        public async Task ExecuteAsync(Job job, CancellationToken token = default)
        {
            Guard.Against.Null(job, nameof(job));

            await semaphoreSlim.WaitAsync(token);
            try
            {
                await job.Execute(token);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}