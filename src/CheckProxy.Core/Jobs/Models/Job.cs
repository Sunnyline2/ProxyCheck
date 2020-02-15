using CheckProxy.Desktop;
using System.Threading;
using System.Threading.Tasks;

namespace DireBlood.Core.Jobs
{
    public abstract class Job : BaseNotifyPropertyChanged
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

        public abstract Task RunAsync(CancellationToken cancellationToken = default);
    }
}
