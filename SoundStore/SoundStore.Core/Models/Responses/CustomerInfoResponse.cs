namespace SoundStore.Core.Models.Responses
{
    /// <summary>
    /// Response model for customer (Admin dashboard)
    /// </summary>
    public class CustomerInfoResponse
    {
        public string Id { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Status { get; set; }
    }

    /// <summary>
    /// Response model for viewing customer's detailed info
    /// </summary>
    public class CustomerDetailedInfoResponse
    {
        public string Id { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? Status { get; set; }
    }
}
