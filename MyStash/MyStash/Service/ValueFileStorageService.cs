using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace MyStash.Service
{
    public class ValueFileStorageService : IValueFileStorageService
    {
        private const string AppDataFolder = "data";
        private const string AppDataExtension = ".dat";

        /// <summary>
        /// Sauvegarder la valeur dans le fichier
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé de la valeur</param>
        /// <param name="value">Valeur</param>
        /// <param name="root"></param>
        /// <returns></returns>
        public async Task SaveValue<T>(string key, T value, IFolder root = null)
        {
            if (value == null)
            {
                await DeleteValue(key);
                return;
            }
            var data = JsonConvert.SerializeObject(value);
            var rootFolder = root ?? FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(AppDataFolder,CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(key + AppDataExtension, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(data);
        }

        /// <summary>
        /// Lire la valeur depuis le fichier
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé</param>
        /// <param name="defaultValue">Default value (optional)</param>
        /// <param name="root"></param>
        /// <returns>Valeur</returns>
        public async Task<T> GetValue<T>(string key, T defaultValue = default (T), IFolder root = null)
        {
            var rootFolder = root ?? FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(AppDataFolder,CreationCollisionOption.OpenIfExists);
            var isFileExisting = await folder.CheckExistsAsync(key + AppDataExtension);

            if (isFileExisting==ExistenceCheckResult.NotFound) return defaultValue;
            try
            {
                var file = await folder.CreateFileAsync(key + AppDataExtension,CreationCollisionOption.OpenIfExists);
                var data = await file.ReadAllTextAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Détruire le fichier valeur
        /// </summary>
        /// <param name="key">Clé</param>
        /// <param name="root"></param>
        public async Task<bool> DeleteValue(string key, IFolder root = null)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var rootFolder = root ?? FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync(AppDataFolder, CreationCollisionOption.OpenIfExists);
            var isFileExisting = await folder.CheckExistsAsync(key + AppDataExtension);
            if (isFileExisting==ExistenceCheckResult.NotFound) return false;
            var file = await folder.CreateFileAsync(key + AppDataExtension, CreationCollisionOption.OpenIfExists);
            await file.DeleteAsync();
            return true;
        }

        public async Task<IFolder> GetFolder(string folderName, IFolder root = null)
        {
            var rootfolder = root ?? FileSystem.Current.LocalStorage;
            return await rootfolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);
        }
    }
}
