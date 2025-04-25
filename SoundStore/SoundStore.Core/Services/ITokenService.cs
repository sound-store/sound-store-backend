using SoundStore.Core.Entities;

namespace SoundStore.Core.Services
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user, string role);
    }
}
