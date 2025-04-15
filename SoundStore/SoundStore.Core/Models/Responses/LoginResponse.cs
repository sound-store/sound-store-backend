namespace SoundStore.Core.Models.Responses
{
    public class LoginResponse
    {
        public string? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }

        /// <summary>
        /// Access's token for user
        /// </summary>
        public string? Token { get; set; }
    }
}
