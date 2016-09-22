using System;
using System.IO;
using MyStash.iOS;
using Xamarin.Forms;
using MyStash.Service;
using SQLite.Net;
using SQLite.Net.Platform.XamarinIOS;

[assembly: Dependency(typeof(DbiOS))]
namespace MyStash.iOS
{
    public class DbiOS : ISqLite
    {
        public SQLiteConnection GetConnection(string databaseName)
        {
            var documentsPath = Environment.GetFolderPath
                 (Environment.SpecialFolder.Personal);
            var libraryPath = Path.Combine(documentsPath, "..", "Library");
            var path = Path.Combine(libraryPath, databaseName);
            var conn = new SQLiteConnection(new SQLitePlatformIOS(), path);
            return conn;
        }
    }
}
