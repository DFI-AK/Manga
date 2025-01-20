using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Mappings;
using Manga.Application.Common.Models;
using Serilog;

namespace Manga.Application.Metrics.Queries.GetHistoricalSystemUsage;

public record GetHistoricalSystemUsageQuery(DateTimeOffset StartDate, DateTimeOffset EndDate) : IRequest<List<SystemUsageModel>>;

public class GetHistoricalSystemUsageQueryValidator : AbstractValidator<GetHistoricalSystemUsageQuery>
{
    public GetHistoricalSystemUsageQueryValidator()
    {
    }
}

public class GetHistoricalSystemUsageQueryHandler(IApplicationDbContext context, ILogger logger, IMapper mapper) : IRequestHandler<GetHistoricalSystemUsageQuery, List<SystemUsageModel>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger _logger = logger.ForContext<GetHistoricalSystemUsageQueryHandler>();
    private readonly IMapper _mapper = mapper;

    public async Task<List<SystemUsageModel>> Handle(GetHistoricalSystemUsageQuery request, CancellationToken cancellationToken)
    {
        List<SystemUsageModel> datapoints = [];
        try
        {
            var (startDate, endDate) = request;
            var history = await _context.SystemUsages
                .Where(x => x.Created >= startDate && x.Created <= endDate)
                .ProjectToListAsync<SystemUsageModel>(_mapper.ConfigurationProvider);

            datapoints.AddRange(history);
            return datapoints;
        }
        catch (Exception ex)
        {
            _logger.Error("An error occured while getting historical data : {message}", ex.Message);
        }
        return datapoints;
    }
}
