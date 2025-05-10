using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using SoundStore.Core.Commons;
using SoundStore.Core.Services;

namespace SoundStore.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            var account = new Account(
                options.Value.CloudName,
                options.Value.ApiKey,
                options.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public virtual Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams, 
            CancellationToken cancellationToken = default)
        {
            return _cloudinary.UploadAsync(uploadParams, cancellationToken);
        }
    }
}
