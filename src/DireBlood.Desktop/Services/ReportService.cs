using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DireBlood.Services
{

    public class StatusService : IStatusService
    {
        public event StatusChanged OnStatusChanged = status => { };

        public void SetStatus(string status)
        {
            OnStatusChanged.Invoke(status);    
        }
    }

    public delegate void StatusChanged(string status);

    public interface IStatusService
    {
        event StatusChanged OnStatusChanged;

        void SetStatus(string status);
    }
}
