using CloudinaryDotNet.Actions;

namespace SoundStore.Core.Services
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadImageAsync(ImageUploadParams uploadParams, 
            CancellationToken cancellationToken = default);
    }
}
