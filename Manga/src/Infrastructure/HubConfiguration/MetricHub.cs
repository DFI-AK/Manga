using AutoMapper;
using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Manga.Infrastructure.HubConfiguration;
public class MetricHub(IApplicationDbContext context, ILogger logger, IMapper mapper, IServiceScopeFactory scope) : Hub<IMetricHub>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger _logger = logger.ForContext<MetricHub>();
    private readonly IMapper _mapper = mapper;
    private readonly ICollectSystemUsage _collectSystemUsage = scope.CreateScope().ServiceProvider.GetRequiredService<ICollectSystemUsage>();
    public override async Task OnConnectedAsync()
    {
        try
        {
            var systemdetail = await _context.SystemDetails.FirstOrDefaultAsync(x => x.MachineName == Environment.MachineName);
            if (systemdetail == null)
            {
                _logger.Warning("No system detail found for the machine: {MachineName}", Environment.MachineName);
                return;
            }
            var systemUsage = _context.SystemUsages.OrderBy(x => x.Created).FirstOrDefaultAsync(x => x.Id == systemdetail.Id);

            var model = _mapper.Map<SystemUsageModel>(systemdetail);

            if (_collectSystemUsage.IsLiveDataEnabled)
            {
                await Clients.All.ReceiveSystemUsage(model);
            }
            await Clients.All.ReceiveRunningStatus(_collectSystemUsage.IsLiveDataEnabled);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occured on connecting to SignalR : {message}", ex.Message);
        }
    }
}
