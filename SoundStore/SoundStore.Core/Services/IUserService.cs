using SoundStore.Core.Commons;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;

namespace SoundStore.Core.Services
{
    public interface IUserService
    {
        Task<PaginatedList<CustomerInfoResponse>> GetCustomers(string name,
            int pageNumber,
            int pageSize);

        Task<CustomerDetailedInfoResponse> GetCustomer(string userId);

        Task<LoginResponse?> Login(string email, string password);

        Task<LoginResponse?> GetUserInfoBasedOnToken();

        Task<bool> RegisterUser(UserRegistration user);

        Task<bool> AddUser(AddedUserRequest request);

        Task<bool> DeleteUser(string userId);

        Task<bool> UpdateStatus(string userId, string status);
    }
}
