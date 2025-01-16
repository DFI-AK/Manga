namespace Manga.Domain.Entities;
public class SystemUsage : BaseEntity<string>
{
    public string? SystemId { get; set; }
    public SystemDetail System { get; set; } = null!;
    public double CpuUsagePercentage { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long NetworkBytesSent { get; set; }
    public long NetworkBytesReceived { get; set; }
    public long DiskReadBytes { get; set; }
    public long DiskWriteBytes { get; set; }
}
