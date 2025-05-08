using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Commons;
using SoundStore.Core.Constants;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using SoundStore.Infrastructure.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace SoundStore.Service
{
    public class UserService(IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IUserClaimsService userClaimsService) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UserService> _logger = logger;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IUserClaimsService _userClaimsService = userClaimsService;

        public async Task<bool> AddUser(AddedUserRequest request)
        {
            try
            {
                var userRepository = _unitOfWork.GetRepository<AppUser>();
                if (request is null)
                    throw new ArgumentException("Invalid requested data!");

                if (!string.Equals(request.Role, UserRoles.Admin, StringComparison.OrdinalIgnoreCase) 
                    && !string.Equals(request.Role, UserRoles.Customer, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Invalid user's role!");
                }

                var existedUser = await _userManager.FindByEmailAsync(request.Email);
                if (existedUser is not null)
                    throw new DuplicatedException("Email has already existed!");

                var user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.Email,
                    NormalizedUserName = request.Email.ToUpper(),
                    Address = request.Address,
                    DateOfBirth = request.DateOfBirth,
                    Status = UserState.Actived,
                };
                var addedResult = await _userManager.CreateAsync(user, request.Password);
                var addedRoleResult = await _userManager.AddToRoleAsync(user, request.Role);

                if (!addedResult.Succeeded || !addedRoleResult.Succeeded)
                {
                    throw new Exception("An error occurred when adding the user!");
                }

                return true;    // Successful operation
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new KeyNotFoundException("User does not exist!");

                if (user.Orders.Any() || user.Transactions.Any())
                {
                    throw new Exception(@"Cannot delete this user 
                        because of data conflict in other tables!");
                }
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    throw new Exception("An error occured while deleting the user!");

                return result.Succeeded;    // Successful operation
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<CustomerDetailedInfoResponse> GetCustomer(string userId)
        {
            var customer = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("Customer not found!");

            return new CustomerDetailedInfoResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Address = customer.Address,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Status = customer.Status.ToString(),
            };
        }

        public async Task<PaginatedList<CustomerInfoResponse>> GetCustomers(string name,
            int pageNumber,
            int pageSize)
        {
            var customers = await _userManager.GetUsersInRoleAsync(UserRoles.Customer);
            if (!customers.Any()) throw new NoDataRetrievalException("No customers found!");
            if (!string.IsNullOrEmpty(name))
            {
                customers = customers.Where(c => c.FirstName!.Contains(name, StringComparison.OrdinalIgnoreCase)
                    || c.LastName!.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var response = customers.Select(x => new CustomerInfoResponse
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                DateOfBirth = x.DateOfBirth,
                Status = x.Status.ToString()
            }).AsQueryable();

            return PaginationHelper.CreatePaginatedList(response, pageNumber, pageSize);
        }

        public async Task<LoginResponse?> GetUserInfoBasedOnToken()
        {
            var userId = _userClaimsService.GetClaim(JwtRegisteredClaimNames.Sid);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User is not authenticated!");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User does not exist!");
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
                ?? throw new KeyNotFoundException("User's role does not exist!");

            return new LoginResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Role = role
            };
        }

        public async Task<LoginResponse?> Login(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email)
                    ?? throw new KeyNotFoundException("User does not exist!");

                var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
                if (!isValidPassword)
                    throw new KeyNotFoundException("Incorrect password!");

                var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
                    ?? string.Empty;
                var token = _tokenService.GenerateToken(user, role);

                return new LoginResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    Token = token,
                    Role = role
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while logging in.");
                throw;
            }
        }

        public async Task<bool> RegisterUser(UserRegistration user)
        {
            try
            {
                if (user.Password != user.ConfirmPassword)
                    throw new ArgumentException("Password and confirm password do not match!");
                
                if (user is null)
                    throw new ArgumentException("User's data is null!");
                
                var duplicateUser = await _userManager.FindByEmailAsync(user.Email ?? string.Empty);
                if (duplicateUser is not null)
                    throw new DuplicatedException("User with this email has already existed!");

                var newUser = new AppUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    NormalizedEmail = user.Email?.ToUpper(),
                    PhoneNumber = user.PhoneNumber,
                    UserName = user.Email,
                    Status = UserState.Actived,
                };

                var addUserResult = await _userManager.CreateAsync(newUser, user.Password
                    ?? string.Empty);
                var addRoleResult = await _userManager.AddToRoleAsync(newUser, UserRoles.Customer);
                if (!addRoleResult.Succeeded || !addUserResult.Succeeded)
                    throw new Exception("An error occurred while adding the customer!");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateStatus(string userId, string status)
        {
            try
            {
                if (!Enum.TryParse(status, true, out UserState currentState))
                {
                    throw new ArgumentException("Invalid user status!");
                }
                var user = await _userManager.FindByIdAsync(userId)
                    ?? throw new KeyNotFoundException("User does not exist!");
                
                user.Status = currentState;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) throw new Exception("An error occurred while updating the user!");

                return result.Succeeded;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw;
            }
        }
    }
}
