using Manga.Application.Common.Models;

namespace Manga.Application.Common.Interfaces;
public interface ISystemProcessingService
{
    SystemDetailModel GetSystemDetail();
    (double CpuUsage, double MemoryUsage, long DiskRead, long DiskWrite) GetSystemUsage();
    (long BytesSent, long BytesReceived) GetNetworkUsage();
}
