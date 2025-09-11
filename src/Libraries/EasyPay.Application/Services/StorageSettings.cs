namespace EasyPay.Application.Services
{
    public class StorageSettings
    {
        public const string SectionName = "StorageSettings";


        public string UploadsFolderPath { get; set; } = "uploads";

        public int MaxFileSizeMB { get; set; } = 5;


        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".pdf" };
    }
}