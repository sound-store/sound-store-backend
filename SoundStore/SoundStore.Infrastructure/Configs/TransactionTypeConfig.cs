﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SoundStore.Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SoundStore.Infrastructure.Configs
{
    [ExcludeFromCodeCoverage]
    public class TransactionTypeConfig : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasIndex(t => t.OrderId)
                .IsUnique();

            builder.HasOne(t => t.Order)
                .WithOne(o => o.Transaction)
                .HasForeignKey<Transaction>(t => t.OrderId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
