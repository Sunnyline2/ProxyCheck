using System;

namespace DireBlood.Core.Jobs.Services
{
    public class JobServiceConfiguration
    {
        public uint MaxThreadsCount { get; }

        public JobServiceConfiguration(uint maxThreadsCount)
        {
            if (maxThreadsCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxThreadsCount));

            MaxThreadsCount = maxThreadsCount;
        }
    }
}