namespace SoundStore.Core.Commons
{
    /// <summary>
    /// Response model for API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public record ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = null!;

        public T? Value { get; set; }
    }
}
