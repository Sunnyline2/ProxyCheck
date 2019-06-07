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
}
