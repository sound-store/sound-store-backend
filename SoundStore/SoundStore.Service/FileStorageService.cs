using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SoundStore.Core.Services;

namespace SoundStore.Service
{
    public class FileStorageService: IFileStorageService
    {
        private readonly ICloudinaryService _cloudinaryService;

        public FileStorageService(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded!");
            }
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "affiliate-network"
            };

            var result = await _cloudinaryService.UploadImageAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }
}
