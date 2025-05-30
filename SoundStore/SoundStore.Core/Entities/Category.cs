﻿using SoundStore.Core.Commons;

namespace SoundStore.Core.Entities
{
    public class Category : AuditableEntity, IEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; } = [];
    }
}
