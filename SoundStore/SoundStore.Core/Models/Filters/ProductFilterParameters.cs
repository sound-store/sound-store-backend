using Microsoft.AspNetCore.Mvc;

namespace SoundStore.Core.Models.Filters
{
    public class ProductFilterParameters
    {
        [BindProperty(Name = "name")]
        public string? Name { get; set; }

        [BindProperty(Name = "categoryId")]
        public int? CategoryId { get; set; }

        [BindProperty(Name = "subCategoryId")]
        public int? SubCategoryId { get; set; }

        [BindProperty(Name = "status")]
        public string? Status { get; set; }
    }
}
