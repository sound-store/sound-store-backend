namespace SoundStore.Core.Models.Responses
{
    public class RatingResponse
    {
        public string UserName { get; set; } = null!;

        public int RatingPoint { get; set; }

        public string? Comment { get; set; }
    }
}
