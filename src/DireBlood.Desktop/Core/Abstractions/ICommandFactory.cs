using System.Windows.Input;

namespace DireBlood.Core.Abstractions
{
    public interface ICommandFactory
    {
        ICommand Get();
    }
}