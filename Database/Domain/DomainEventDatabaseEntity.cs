namespace Codelet.Database.Domain
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using Codelet.Database.Entities;
  using Codelet.Domain;
  using Microsoft.Extensions.Logging;
  using Newtonsoft.Json;

  /// <summary>
  /// Database entity for domain event.
  /// </summary>
  public class DomainEventDatabaseEntity : DatabaseEntity<Guid>
  {
    private static readonly JsonSerializerSettings Settings
      = new JsonSerializerSettings
      {
        TypeNameHandling = TypeNameHandling.Objects,
      };

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventDatabaseEntity" /> class.
    /// </summary>
    [Obsolete(ForEntityFrameworkOnlyObsoleteReason)]
    protected DomainEventDatabaseEntity()
    {
    }

    private DomainEventDatabaseEntity(DateTimeOffset occured, ISerializedDomainEvent content)
    {
      this.Id = Guid.NewGuid();
      this.Occured = occured;
      this.Content = JsonConvert.SerializeObject(content, Settings);
    }

    /// <summary>
    /// Internal non-generic interface for serialized event.
    /// </summary>
    private interface ISerializedDomainEvent
    {
      /// <summary>
      /// Deserializes the domain event into domain event dispatcher using the <paramref name="factory"/> .
      /// </summary>
      /// <param name="factory">The factory.</param>
      /// <returns>The domain event dispatcher.</returns>
      DomainEventDispatcher Deserialize(IDomainEventsDispatcherFactory factory);
    }

    /// <summary>
    /// Gets the moment when event occured.
    /// </summary>
    [Required]
    public DateTimeOffset Occured { get; private set; }

    /// <summary>
    /// Gets serialized event content.
    /// </summary>
    [Required]
    public string Content { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainEventDatabaseEntity" /> class.
    /// </summary>
    /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
    /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
    /// <param name="occured">The moment when the domain event occured.</param>
    /// <param name="databaseEntityId">The database entity identifier.</param>
    /// <param name="serializedDomainArgs">The serialized domain arguments.</param>
    /// <returns>The domain event database entity.</returns>
    public static DomainEventDatabaseEntity Create<TDomainModel, TDomainEventArgs, TDatabaseEntity, TDatabaseEntityId>(
      DateTimeOffset occured,
      TDatabaseEntityId databaseEntityId,
      string serializedDomainArgs)
      where TDomainModel : DomainModel
      where TDomainEventArgs : DomainEventArgs<TDomainModel>
      where TDatabaseEntity : class, IDatabaseEntityWithId<TDatabaseEntityId>, IDatabaseEntityWithModel<TDomainModel>, IDatabaseEntity
      => new DomainEventDatabaseEntity(
        occured,
        new SerializedDomainEvent<TDomainModel, TDomainEventArgs, TDatabaseEntity, TDatabaseEntityId>(
          databaseEntityId,
          serializedDomainArgs));

    /// <summary>
    /// Deserializes this domain event database entity into domain event dispatcher
    /// using given <paramref name="factory"/>.
    /// </summary>
    /// <param name="factory">The dispatcher factory.</param>
    /// <returns>The domain event dispatcher.</returns>
    public DomainEventDispatcher Deserialize(IDomainEventsDispatcherFactory factory)
      => JsonConvert.DeserializeObject<ISerializedDomainEvent>(this.Content, Settings).Deserialize(factory);

    /// <inheritdoc />
    public override void Synchronize(ILogger logger)
    {
    }

    /// <summary>
    /// Generic serialized domain event type.
    /// </summary>
    /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
    /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
    /// <seealso cref="Codelet.Database.Domain.DomainEventDatabaseEntity.ISerializedDomainEvent" />
    private class SerializedDomainEvent<TDomainModel, TDomainEventArgs, TDatabaseEntity, TDatabaseEntityId>
      : ISerializedDomainEvent
      where TDomainModel : DomainModel
      where TDomainEventArgs : DomainEventArgs<TDomainModel>
      where TDatabaseEntity : class, IDatabaseEntityWithId<TDatabaseEntityId>, IDatabaseEntityWithModel<TDomainModel>, IDatabaseEntity
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="SerializedDomainEvent{TDomainModel, TDomainEventArgs, TDatabaseEntity, TDatabaseEntityId}"/> class.
      /// </summary>
      /// <param name="databaseEntityId">The database entity identifier.</param>
      /// <param name="serializedDomainArgs">The serialized domain arguments.</param>
      public SerializedDomainEvent(TDatabaseEntityId databaseEntityId, string serializedDomainArgs)
      {
        this.DatabaseEntityId = databaseEntityId;
        this.SerializedDomainArgs = serializedDomainArgs;
      }

      /// <summary>
      /// Gets the database entity identifier.
      /// </summary>
      // ReSharper disable once MemberCanBePrivate.Local
      public TDatabaseEntityId DatabaseEntityId { get; }

      /// <summary>
      /// Gets the serialized domain arguments.
      /// </summary>
      // ReSharper disable once MemberCanBePrivate.Local
      public string SerializedDomainArgs { get; }

      /// <inheritdoc />
      public DomainEventDispatcher Deserialize(IDomainEventsDispatcherFactory factory)
      {
        return factory.Create<TDomainModel, TDomainEventArgs, TDatabaseEntity, TDatabaseEntityId>(
          this.DatabaseEntityId,
          this.SerializedDomainArgs);
      }
    }
  }
}