using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class CategoryUpdatedRequest
    {
        [Required(ErrorMessage = "Category name is required", AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;
    }
}
