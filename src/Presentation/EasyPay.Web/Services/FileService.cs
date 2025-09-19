using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;

namespace EasyPay.Web.Services
{
    public interface IFileService
    {
        Task<string?> UploadFileAsync(IBrowserFile file, string subfolder);
    }
    public class FileService : IFileService
    {
        private readonly HttpClient _httpClient;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> UploadFileAsync(IBrowserFile file, string subfolder)
        {
            if (file == null)
            {
                return null;
            }

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenReadStream(MaxFileSize));
            content.Add(content: fileContent, name: "\"file\"", fileName: file.Name);

            var response = await _httpClient.PostAsync(ApiEndpoints.Files.UploadDocument, content);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<UploadResponse>();
            return result?.Url;
        }

        private record UploadResponse(string Url);
    }
}
