namespace SoundStore.Core.Commons
{
    public record ErrorResponse
    {
        public bool IsSuccess { get; set; } = false;

        public string Message { get; set; } = string.Empty;

        public string? Details { get; set; }
    }
}
