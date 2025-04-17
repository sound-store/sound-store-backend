using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    /// <summary>
    /// Request model for adding user (Admin)
    /// </summary>
    public class AddedUserRequest
    {
        [Required(ErrorMessage = "First name is required!", AllowEmptyStrings = false)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required!", AllowEmptyStrings = false)]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Default password
        /// </summary>
        public string Password { get; set; } = "1234aBcD@";

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Role is required!", AllowEmptyStrings = false)]
        public string Role { get; set; } = null!;
    }
}
