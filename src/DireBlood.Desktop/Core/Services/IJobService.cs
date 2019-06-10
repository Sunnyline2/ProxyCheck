using System.Threading.Tasks;

namespace DireBlood.Core.Services
{
    public interface IJobService
    {
        bool IsRunning { get; }
        Task ExecuteAsync<T>(JobAsync<T> job) where T : class, new();
        void Execute<T>(Job<T> job) where T : class, new();
    }
}