using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SoundStore.Core.Commons;
using SoundStore.Core.Services;

namespace SoundStore.Service
{
    public class ImageService: IImageService
    {
        private readonly Cloudinary _cloudinary;

        public ImageService(IOptions<CloudinarySettings> options)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded!");
            }
            var uploadResult = new ImageUploadResult();
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "affiliate-network",
                    Transformation = new Transformation().Quality(85).FetchFormat("auto")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);
            var imageUrl = uploadResult.SecureUrl.ToString();
            return imageUrl;
        }
    }
}
