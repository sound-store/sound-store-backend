using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; init; } = null!;

        [Required(ErrorMessage = "Password is required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; init; } = null!;
    }
}
