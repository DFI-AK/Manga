
using Manga.Application.Common.Models;
using Manga.Application.Metrics.Commands.ToggleLiveSystemUsageData;
using Manga.Application.Metrics.Queries.GetHistoricalSystemUsage;
using Microsoft.AspNetCore.Mvc;

namespace Manga.Web.Endpoints;

public class Metrics : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(ToggleLiveMetricData, $"{nameof(ToggleLiveMetricData)}/{{toggleLiveData}}")
            .MapGet(GetHistoricalDataOfSystemUsage, $"{nameof(GetHistoricalDataOfSystemUsage)}")
            ;
    }

    public async Task<Result> ToggleLiveMetricData(ISender sender, bool toggleLiveData) => await sender.Send(new ToggleLiveSystemUsageDataCommand(toggleLiveData));
    public async Task<List<SystemUsageModel>> GetHistoricalDataOfSystemUsage(ISender sender, [FromQuery] DateTimeOffset startDate, DateTimeOffset endDate) => await sender.Send(new GetHistoricalSystemUsageQuery(startDate, endDate));
}
