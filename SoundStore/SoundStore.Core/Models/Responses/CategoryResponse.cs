namespace SoundStore.Core.Models.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = null!;
        
        public List<SubCategoryResponse> SubCategories { get; set; } = [];
    }

    public class SubCategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        
        public int? CategoryId { get; set; }
    }
}
