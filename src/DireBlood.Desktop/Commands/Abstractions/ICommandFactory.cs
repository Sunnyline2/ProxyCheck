namespace DireBlood.Commands.Abstractions
{
    public interface ICommandFactory
    {
        RelayCommand GetCommand();
    }
}