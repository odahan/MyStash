namespace MyStash.Service
{
    public interface ILoginSwitch
    {
        void ShowMainPage();
        void LogOut();
        void CreatePassword();
        void ResetTimeout();
    }
}
