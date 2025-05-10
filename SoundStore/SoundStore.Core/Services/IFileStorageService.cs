using Microsoft.AspNetCore.Http;

namespace SoundStore.Core.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadImage(IFormFile file);
    }
}
