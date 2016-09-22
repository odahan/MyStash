using SQLite.Net;

namespace MyStash.Service
{
    public interface ISqLite
    {
        SQLiteConnection GetConnection(string databaseName);
    }
}
