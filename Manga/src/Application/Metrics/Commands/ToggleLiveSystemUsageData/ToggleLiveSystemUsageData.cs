using Manga.Application.Common.Interfaces;
using Manga.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Manga.Application.Metrics.Commands.ToggleLiveSystemUsageData;

public record ToggleLiveSystemUsageDataCommand(bool ToggleEnableLiveData) : IRequest<Result>;

public class ToggleLiveSystemUsageDataCommandValidator : AbstractValidator<ToggleLiveSystemUsageDataCommand>
{
    public ToggleLiveSystemUsageDataCommandValidator()
    {
        RuleFor(x => x.ToggleEnableLiveData).NotNull().WithMessage("The toggle value from client is required.");
    }
}

public class ToggleLiveSystemUsageDataCommandHandler(IServiceScopeFactory scope) : IRequestHandler<ToggleLiveSystemUsageDataCommand, Result>
{
    private readonly ICollectSystemUsage _collectSystemUsage = scope.CreateScope().ServiceProvider.GetRequiredService<ICollectSystemUsage>();

    public Task<Result> Handle(ToggleLiveSystemUsageDataCommand request, CancellationToken cancellationToken)
    {
        _collectSystemUsage.HandleEnableLiveData(request.ToggleEnableLiveData);
        return Task.FromResult(Result.Success());
    }
}
