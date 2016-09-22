using System.Threading.Tasks;
using PCLStorage;

namespace MyStash.Service
{
    public interface IValueFileStorageService
    {
        /// <summary>
        /// Sauvegarder la valeur dans le fichier
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé de la valeur</param>
        /// <param name="value">Valeur</param>
        /// <param name="root">Folder to store data default local when null</param>
        /// <returns></returns>
        Task SaveValue<T>(string key, T value, IFolder root = null);

        /// <summary>
        /// Lire la valeur depuis le fichier
        /// </summary>
        /// <typeparam name="T">Type de la valeur</typeparam>
        /// <param name="key">Clé</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="root">Folder to get data default to local when null</param>
        /// <returns>Valeur</returns>
        Task<T> GetValue<T>(string key,T defaultValue = default(T), IFolder root = null);

        /// <summary>
        /// Détruire le fichier valeur
        /// </summary>
        /// <param name="key">Clé</param>
        /// <param name="root">Folder where the key must be deleted, defaut to local when null</param>
        Task<bool> DeleteValue(string key, IFolder root = null);

        /// <summary>
        /// Retourner un IFolder
        /// </summary>
        /// <param name="folderName">nom du dossier</param>
        /// <param name="root">Root where to get the folder</param>
        /// <returns>IFolder</returns>
        Task<IFolder> GetFolder(string folderName, IFolder root = null);
    }
}