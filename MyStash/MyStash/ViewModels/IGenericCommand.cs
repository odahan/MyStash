namespace MyStash.ViewModels
{
    public interface IGenericCommand
    {
        void SendCommand(string commandName, object context = null);
    }
}
