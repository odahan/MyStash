using System.IO;
using Windows.Storage;
using MyStash.Service;
using MyStash.WinPhone;
using SQLite.Net.Platform.WinRT;
using Xamarin.Forms;

[assembly: Dependency(typeof(DbWP))]
namespace MyStash.WinPhone
{

    public class DbWP : ISqLite
    {

        #region ISQLite implementation
        public SQLite.Net.SQLiteConnection GetConnection(string databaseName)
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseName);
            var conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), path);
            return conn;
        }
        #endregion
    }
}