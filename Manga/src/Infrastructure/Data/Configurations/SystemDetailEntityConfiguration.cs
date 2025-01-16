using Manga.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manga.Infrastructure.Data.Configurations;
public class SystemDetailEntityConfiguration : IEntityTypeConfiguration<SystemDetail>
{
    public void Configure(EntityTypeBuilder<SystemDetail> builder)
    {
        builder.Property(x => x.Id).IsRequired().HasDefaultValueSql("NEWID()");
    }
}
