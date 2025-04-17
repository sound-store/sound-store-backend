namespace SoundStore.Core.Models.Responses
{
    public class ProductRatingResponse
    {
        public long ProductId { get; set; }

        /// <summary>
        /// The average rating point of a product
        /// </summary>
        public decimal RatingPoint { get; set; }

        public List<string> Comment { get; set; } = [];
    }
}
