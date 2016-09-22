using System.IO;
using MyStash.Service;
using MyStash.UWP;
using SQLite.Net.Platform.WinRT;
using Xamarin.Forms;

[assembly: Dependency(typeof(DbUWP))]
namespace MyStash.UWP
{

    public class DbUWP : ISqLite
    {

        #region ISQLite implementation
        public SQLite.Net.SQLiteConnection GetConnection(string databaseName)
        {
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, databaseName);
            var conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            return conn;
        }
        #endregion
    }
}