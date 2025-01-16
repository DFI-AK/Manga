using Manga.Domain.Entities;

namespace Manga.Application.Common.Models;
public record SystemUsageModel
{
    public double CpuUsagePercentage { get; set; }
    public double MemoryUsagePercentage { get; set; }
    public long NetworkBytesSent { get; set; }
    public long NetworkBytesReceived { get; set; }
    public long DiskReadBytes { get; set; }
    public long DiskWriteBytes { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SystemUsage, SystemUsageModel>()
                .ForMember(dest => dest.TimeStamp, o => o.MapFrom(src => src.Created));
        }
    }
}
