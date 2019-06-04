namespace CheckProxy.Desktop.Commands.Abstractions
{
    public interface ICommandFactory
    {
        RelayCommand GetCommand();
    }
}