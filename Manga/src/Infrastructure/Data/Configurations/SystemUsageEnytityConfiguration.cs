using Manga.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manga.Infrastructure.Data.Configurations;
public class SystemUsageEnytityConfiguration : IEntityTypeConfiguration<SystemUsage>
{
    public void Configure(EntityTypeBuilder<SystemUsage> builder)
    {
        builder.Property(x => x.Id).IsRequired().HasDefaultValueSql("NEWID()");

        builder.HasOne(x => x.System)
            .WithMany(x => x.SystemUsages)
            .HasForeignKey(x => x.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
