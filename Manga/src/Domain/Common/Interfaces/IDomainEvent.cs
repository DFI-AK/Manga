namespace Manga.Domain.Common.Interfaces;
public interface IDomainEvent
{
    IReadOnlyCollection<BaseEvent> DomainEvents { get; }
    void AddDomainEvent(BaseEvent baseEvent);
    void RemoveDomainEvent(BaseEvent baseEvent);
    void ClearDomainEvents();
}
