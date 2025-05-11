using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStore.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Infrastructure.Configs
{
    [ExcludeFromCodeCoverage]
    public class SubCategoryTypeConfig : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            builder.HasOne(sc => sc.Category)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(sc => sc.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
