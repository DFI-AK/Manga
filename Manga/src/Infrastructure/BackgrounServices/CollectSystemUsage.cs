using AutoMapper;
using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Models;
using Manga.Domain.Entities;
using Manga.Infrastructure.Data;
using Manga.Infrastructure.HubConfiguration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Manga.Infrastructure.BackgrounServices;
internal sealed class CollectSystemUsage(IServiceScopeFactory scope, IHubContext<MetricHub, IMetricHub> hubContext, IMapper mapper) : BackgroundService, ICollectSystemUsage
{
    private readonly ILogger _logger = scope.CreateScope().ServiceProvider.GetRequiredService<ILogger>().ForContext<CollectSystemUsage>();
    private readonly ISystemProcessingService _systemProcessingService = scope.CreateScope().ServiceProvider.GetRequiredService<ISystemProcessingService>();
    private readonly ApplicationDbContext _context = scope.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
    private readonly IHubContext<MetricHub, IMetricHub> _hubContext = hubContext;
    private readonly IMapper _mapper = mapper;

    private bool _isLiveDataEnabled { get; set; } = true;

    public bool IsLiveDataEnabled => _isLiveDataEnabled;

    public void HandleEnableLiveData(bool enableLiveData)
    {
        _isLiveDataEnabled = enableLiveData;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information("Start collecting system usage");

        try
        {
            var startTime = DateTime.UtcNow;
            var systemDetail = await _context.SystemDetails.FirstOrDefaultAsync(x => x.MachineName == Environment.MachineName, cancellationToken: stoppingToken);

            if (systemDetail == null)
            {
                _logger.Warning("No system detail found for the machine: {MachineName}", Environment.MachineName);
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var (CpuUsage, MemoryUsage, DiskRead, DiskWrite) = _systemProcessingService.GetSystemUsage();
                var (BytesSent, BytesReceived) = _systemProcessingService.GetNetworkUsage();
                var details = _systemProcessingService.GetSystemDetail();

                var usageEntity = new SystemUsage()
                {
                    SystemId = systemDetail.Id,
                    CpuUsagePercentage = CpuUsage,
                    MemoryUsagePercentage = MemoryUsage,
                    DiskReadBytes = DiskRead,
                    DiskWriteBytes = DiskWrite,
                    NetworkBytesReceived = BytesReceived,
                    NetworkBytesSent = BytesSent,
                };

                await _context.SystemUsages.AddAsync(usageEntity, stoppingToken);

                await _context.SaveChangesAsync(stoppingToken);

                if (IsLiveDataEnabled)
                {
                    SystemUsageModel model = _mapper.Map<SystemUsageModel>(usageEntity);
                    await _hubContext.Clients.All.ReceiveSystemUsage(model);
                }

                await _hubContext.Clients.All.ReceiveRunningStatus(IsLiveDataEnabled);
                await _hubContext.Clients.All.GetSystenDetails(new()
                {
                    Architecture = details.Architecture,
                    FreeMemoryKB = details.FreeMemoryKB,
                    MachineName = details.MachineName,
                    OSVersion = details.OSVersion,
                    ProcessorCount = details.ProcessorCount,
                    TotalMemoryKB = details.TotalMemoryKB
                });

                _logger.Information("Metrics collected successfully.");

                var elapsedTime = DateTime.UtcNow - startTime;
                var delay = TimeSpan.FromSeconds(5) - elapsedTime;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error("An error occured while collecting system usage : {message}", ex.Message);
        }
    }
}
