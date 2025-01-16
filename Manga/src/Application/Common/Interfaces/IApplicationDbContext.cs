using Manga.Domain.Entities;

namespace Manga.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<SystemUsage> SystemUsages { get; }

    DbSet<SystemDetail> SystemDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
