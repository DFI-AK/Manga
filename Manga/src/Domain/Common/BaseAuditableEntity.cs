using Manga.Domain.Common.Interfaces;

namespace Manga.Domain.Common;

public abstract class BaseAuditableEntity : IAuditableEntity
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
