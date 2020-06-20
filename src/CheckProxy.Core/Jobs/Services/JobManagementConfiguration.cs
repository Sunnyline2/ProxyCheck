namespace DireBlood.Core.Jobs.Services
{
    public class JobManagementConfiguration
    {
        public int MaxThreadsCount { get; }

        public JobManagementConfiguration(int maxThreadsCount)
        {
            Guard.Against.NegativeOrZero(maxThreadsCount, nameof(maxThreadsCount));

            MaxThreadsCount = maxThreadsCount;
        }
    }
}