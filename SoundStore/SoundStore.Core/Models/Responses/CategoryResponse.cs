namespace SoundStore.Core.Models.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<SubCategoryResponse> SubCategories { get; set; } = [];
    }

    public class SubCategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        
        public int? CategoryId { get; set; }
    }
}
