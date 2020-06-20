using CheckProxy.Desktop;
using DireBlood.Core.Jobs.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DireBlood.Core.Jobs
{
    public class Job : BaseViewModel
    {
        private string title;

        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        private string description;

        public string Description
        {
            get => description;
            set => Set(ref description, value);
        }

        private JobStatus status;

        public JobStatus Status
        {
            get => status;
            set => Set(ref status, value);
        }

        private readonly Func<Job, CancellationToken, Task> func;

        public Job(Func<Job, CancellationToken, Task> func)
        {
            Guard.Against.Null(func, nameof(func));

            this.func = func;
        }

        public async Task Execute(CancellationToken token = default)
        {
            await func(this, token).ConfigureAwait(false);
        }
    }
}
