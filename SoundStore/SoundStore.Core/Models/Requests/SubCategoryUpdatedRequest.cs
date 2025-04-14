using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class SubCategoryUpdatedRequest
    {
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Sub category name is required", AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;
    }
}
