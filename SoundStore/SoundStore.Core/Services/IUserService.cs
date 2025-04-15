using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;

namespace SoundStore.Core.Services
{
    public interface IUserService
    {
        Task<LoginResponse?> Login(string email, string password);

        Task<LoginResponse?> GetProfile();

        Task<bool> RegisterUser(UserRegistration user);
    }
}
