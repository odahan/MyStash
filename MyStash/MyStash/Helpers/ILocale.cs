namespace MyStash.Helpers
{
    public interface ILocale
    {
        string GetCurrent();

        void SetLocale(string culture = "");
    }
}
