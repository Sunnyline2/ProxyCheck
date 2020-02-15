using System;
using System.Collections.Generic;
using System.Text;

namespace DireBlood.Core.Jobs.Services
{
    public class JobService
    {
        private readonly JobServiceConfiguration configuration;

        public JobService(JobServiceConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}