using Manga.Application.Common.Models;

namespace Manga.Application.Common.Interfaces;
public interface IMetricHub
{
    Task ReceiveSystemUsage(SystemUsageModel model);
    Task ReceiveRunningStatus(bool isLive);
    Task GetOldData(List<SystemUsageModel> model);
}
