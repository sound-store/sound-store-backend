using System.ComponentModel.DataAnnotations;

namespace SoundStore.Core.Models.Requests
{
    public class SubCategoryCreatedRequest
    {
        [Required(ErrorMessage = "Subcategory name is required", AllowEmptyStrings = false)]
        public string Name { get; set; } = null!;
    }
}
