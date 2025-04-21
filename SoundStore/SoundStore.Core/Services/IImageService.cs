using Microsoft.AspNetCore.Http;

namespace SoundStore.Core.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
