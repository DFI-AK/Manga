using Manga.Domain.Entities;

namespace Manga.Application.Common.Models;
public record SystemDetailModel
{
    public string? MachineName { get; set; }
    public string? OSVersion { get; set; }
    public int ProcessorCount { get; set; }
    public string? Architecture { get; set; }
    public long TotalMemoryKB { get; set; }
    public long FreeMemoryKB { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<SystemDetail, SystemDetailModel>();
        }
    }
}
