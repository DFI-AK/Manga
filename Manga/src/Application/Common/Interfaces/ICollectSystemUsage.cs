using Microsoft.Extensions.Hosting;

namespace Manga.Application.Common.Interfaces;
public interface ICollectSystemUsage : IHostedService, IDisposable
{
    public bool IsLiveDataEnabled { get; }
    void HandleEnableLiveData(bool enableLiveData);
}
