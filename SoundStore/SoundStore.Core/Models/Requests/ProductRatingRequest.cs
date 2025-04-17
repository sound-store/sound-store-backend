using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class ProductRatingRequest
    {
        [Required(ErrorMessage = "Product id is required!")]
        public long ProductId { get; set; }

        /// <summary>
        /// Rating point from 1 to 5 of a product
        /// </summary>
        [Required(ErrorMessage = "Please rating the product from 1 to 5!")]
        public int Point { get; set; }

        public string? Comment { get; set; }
    }
}
