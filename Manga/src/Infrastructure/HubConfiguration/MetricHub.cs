using AutoMapper;
using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Mappings;
using Manga.Application.Common.Models;
using Manga.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Manga.Infrastructure.HubConfiguration;

[Authorize(AuthenticationSchemes = Env.AuthorizeSchema)]
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
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                _logger.Information("User id from Hub : {userId}", userId);
            }

            var systemdetail = await _context.SystemDetails.FirstOrDefaultAsync(x => x.MachineName == Environment.MachineName);
            if (systemdetail == null)
            {
                _logger.Warning("No system detail found for the machine: {MachineName}", Environment.MachineName);
                return;
            }
            var systemUsage = await _context.SystemUsages
                .Where(x => x.Created >= DateTimeOffset.UtcNow.AddMinutes(-6))
                .OrderBy(x => x.Created).ProjectToListAsync<SystemUsageModel>(_mapper.ConfigurationProvider);

            if (_collectSystemUsage.IsLiveDataEnabled)
            {
                await Clients.All.GetOldData(systemUsage);
            }
            await Clients.All.ReceiveRunningStatus(_collectSystemUsage.IsLiveDataEnabled);
        }
        catch (Exception ex)
        {
            _logger.Error("An error occured on connecting to SignalR : {message}", ex.Message);
        }
    }
}
