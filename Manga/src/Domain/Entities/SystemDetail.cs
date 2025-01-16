namespace Manga.Domain.Entities;
public class SystemDetail : BaseEntity<string>
{
    public string? MachineName { get; set; }
    public string? OSVersion { get; set; }
    public int ProcessorCount { get; set; }
    public string? Architecture { get; set; }
    public long TotalMemoryKB { get; set; }
    public long FreeMemoryKB { get; set; }
    public ICollection<SystemUsage> SystemUsages { get; set; } = [];
}
