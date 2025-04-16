using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SoundStore.Core;
using SoundStore.Core.Constants;
using SoundStore.Core.Entities;
using SoundStore.Core.Enums;
using SoundStore.Core.Exceptions;
using SoundStore.Core.Models.Requests;
using SoundStore.Core.Models.Responses;
using SoundStore.Core.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SoundStore.Service
{
    public class UserService(IUnitOfWork unitOfWork,
        ILogger<UserService> logger,
        UserManager<AppUser> userManager,
        TokenService tokenService,
        UserClaimsService userClaimsService) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<UserService> _logger = logger;
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly TokenService _tokenService = tokenService;
        private readonly UserClaimsService _userClaimsService = userClaimsService;

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
                var userRepository = _unitOfWork.GetRepository<AppUser>();

                if (user.Password != user.ConfirmPassword)
                    throw new ArgumentException("Password and confirm password do not match!");
                if (user is null)
                    throw new ArgumentException("User's data is null!");

                var isEmailDuplicate = await userRepository.GetAll()
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == user.Email);
                if (isEmailDuplicate)
                    throw new DuplicatedException("Email already exists!");

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
                if (!addUserResult.Succeeded)
                    throw new Exception("User registration failed!");

                var addRoleResult = await _userManager.AddToRoleAsync(newUser, UserRoles.Customer);
                if (!addRoleResult.Succeeded)
                    throw new Exception("Adding role to user failed!");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}
