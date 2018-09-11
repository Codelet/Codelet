namespace Codelet.Database.Domain
{
  using System.Collections.Generic;

  /// <summary>
  /// The database entity for domain model.
  /// </summary>
  public interface IDomainModelDatabaseEntity
  {
    /// <summary>
    /// Transfers domain events.
    /// </summary>
    /// <param name="serializer">The domain event serializer.</param>
    /// <returns>The domain event database entities.</returns>
    IEnumerable<DomainEventDatabaseEntity> TransferDomainEvents(IDomainEventsSerializer serializer);
  }
}