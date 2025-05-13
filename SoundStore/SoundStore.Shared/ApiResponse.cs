namespace SoundStore.Shared
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        
        public required string Message { get; set; }

        public T? Data { get; set; }
    }
}
