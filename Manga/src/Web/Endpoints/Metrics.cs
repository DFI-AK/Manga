
using Manga.Application.Common.Models;
using Manga.Application.Metrics.Commands.ToggleLiveSystemUsageData;

namespace Manga.Web.Endpoints;

public class Metrics : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(ToggleLiveMetricData, $"{nameof(ToggleLiveMetricData)}/{{toggleLiveData}}");
    }

    public async Task<Result> ToggleLiveMetricData(ISender sender, bool toggleLiveData) => await sender.Send(new ToggleLiveSystemUsageDataCommand(toggleLiveData));
}
