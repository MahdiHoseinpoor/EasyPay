using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EasyPay.Application.Services
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves a file to the configured storage and returns a unique identifier or URL.
        /// </summary>
        /// <param name="file">The IFormFile to be saved.</param>
        /// <param name="subfolder">An optional subfolder to organize files (e.g., "user-documents").</param>
        /// <returns>The publicly accessible URL of the stored file.</returns>
        Task<string> SaveFileAsync(IFormFile file, string subfolder);

        /// <summary>
        /// Deletes a file from storage based on its public URL.
        /// </summary>
        /// <param name="fileUrl">The publicly accessible URL of the file to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteFileAsync(string fileUrl);
    }
}