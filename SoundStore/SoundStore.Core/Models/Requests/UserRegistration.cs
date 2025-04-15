using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class UserRegistration
    {
        [Required(ErrorMessage = "First name is required!", AllowEmptyStrings = false)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required!", AllowEmptyStrings = false)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Address is required!", AllowEmptyStrings = false)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Date of birth is required!")]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required!", AllowEmptyStrings = false)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        //[BindProperty(Name = "password")]
        [Required(ErrorMessage = "Password is required!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Length(8, 25, ErrorMessage = "Password must be between 8 and 25 characters long.")]
        [RegularExpression(@"^(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Password must contain at least one special character.")]
        public string? Password { get; set; }

        //[BindProperty(Name = "confirmPassword")]
        [Required(ErrorMessage = "Please enter the password again!", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Length(8, 25, ErrorMessage = "Password must be between 8 and 25 characters long.")]
        public string? ConfirmPassword { get; set; }
    }
}
