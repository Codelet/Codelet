namespace Codelet.Database.Domain
{
  using Codelet.Database.Entities;
  using Codelet.Domain;

  /// <summary>
  /// Creates domain events dispatchers.
  /// </summary>
  public interface IDomainEventsDispatcherFactory
  {
    /// <summary>
    /// Creates the domain event dispatcher for the given database entity id and serialized event data.
    /// </summary>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
    /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
    /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
    /// <param name="databaseEntityId">The database entity identifier.</param>
    /// <param name="serializedDomainEvent">The serialized domain event.</param>
    /// <returns>The domain event dispatcher.</returns>
    DomainEventDispatcher Create<TDatabaseEntity, TDatabaseEntityId, TDomainModel, TDomainEventArgs>(
      TDatabaseEntityId databaseEntityId,
      string serializedDomainEvent)
      where TDatabaseEntity : class, IDatabaseEntityWithId<TDatabaseEntityId>, IDatabaseEntityWithModel<TDomainModel>
      where TDomainModel : DomainModel
      where TDomainEventArgs : DomainEventArgs<TDomainModel>;
  }
}