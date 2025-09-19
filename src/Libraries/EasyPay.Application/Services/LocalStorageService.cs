using EasyPay.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Services
{
    public class LocalStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StorageSettings _storageSettings;

        public LocalStorageService(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor,
            IOptions<StorageSettings> storageSettings)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _storageSettings = storageSettings.Value;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder)
        {
            ValidateFile(file);

            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                throw new InvalidOperationException("WebRootPath is not configured. Ensure the 'wwwroot' folder exists in the startup project (EasyPay.Api).");
            }
            var uploadsRootFolder = Path.Combine(_env.WebRootPath, _storageSettings.UploadsFolderPath);
            var targetFolder = Path.Combine(uploadsRootFolder, subfolder);
            Directory.CreateDirectory(targetFolder); 

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, uniqueFileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var request = _httpContextAccessor.HttpContext!.Request;
            var url = $"{request.Scheme}://{request.Host}/{_storageSettings.UploadsFolderPath}/{subfolder}/{uniqueFileName}";

            return url.Replace("\\", "/");
        }

        public Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return Task.CompletedTask;
            }

            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}/";

            if (!fileUrl.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return Task.CompletedTask;
            }

            var relativePath = fileUrl.Substring(baseUrl.Length);
            var physicalPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", "\\"));

            try
            {
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                }
            }
            catch (Exception ex)
            {
            }

            return Task.CompletedTask;
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.");
            }

            var maxFileSizeBytes = _storageSettings.MaxFileSizeMB * 1024 * 1024;
            if (file.Length > maxFileSizeBytes)
            {
                throw new InvalidOperationException($"File size exceeds the limit of {_storageSettings.MaxFileSizeMB} MB.");
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_storageSettings.AllowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"File type '{fileExtension}' is not allowed.");
            }
        }
    }
}