using System.IO;
using MyStash.Droid;
using MyStash.Service;
using SQLite.Net.Platform.XamarinAndroid;

[assembly: Xamarin.Forms.Dependency(typeof(DbDroid))]
namespace MyStash.Droid
{

    public class DbDroid : ISqLite
    {

        #region ISQLite implementation
        public SQLite.Net.SQLiteConnection GetConnection(string databaseName)
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, databaseName);
            var conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformAndroid(), path);
            return conn;
        }
        #endregion
    }
}