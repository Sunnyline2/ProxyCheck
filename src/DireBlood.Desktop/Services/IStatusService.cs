namespace DireBlood.Services
{
    public interface IStatusService
    {
        event StatusChanged OnStatusChanged;

        void SetStatus(string status);
    }
}
