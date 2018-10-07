namespace Codelet.Database.Domain
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using Codelet.Database.Entities;
  using Codelet.Domain;

  /// <summary>
  /// The database entity for domain models base class.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public abstract class DomainModelDatabaseEntity<TIdentifier, TModel>
    : DatabaseEntityWithModel<TIdentifier, TModel>, IDomainModelDatabaseEntity
    where TModel : DomainModel
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainModelDatabaseEntity{TIdentifier, TModel}"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    protected DomainModelDatabaseEntity(TModel model)
      : base(model)
    {
      this.Timer.Start();
      this.SkipFirstEventsTransfer = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainModelDatabaseEntity{TIdentifier, TModel}"/> class.
    /// </summary>
    [Obsolete(ForEntityFrameworkOnlyObsoleteReason)]
    protected DomainModelDatabaseEntity()
    {
      this.Timer.Start();
      this.SkipFirstEventsTransfer = false;
    }

    private bool SkipFirstEventsTransfer { get; set; }

    private DateTimeOffset Created { get; } = DateTimeOffset.UtcNow;

    private Stopwatch Timer { get; } = new Stopwatch();

    private ConcurrentBag<Func<IDomainEventsSerializer, DomainEventDatabaseEntity>> DomainEvents { get; }
      = new ConcurrentBag<Func<IDomainEventsSerializer, DomainEventDatabaseEntity>>();

    /// <inheritdoc />
    IEnumerable<DomainEventDatabaseEntity> IDomainModelDatabaseEntity.TransferDomainEvents(
      IDomainEventsSerializer serializer)
    {
      if (this.SkipFirstEventsTransfer)
      {
        this.SkipFirstEventsTransfer = false;
        yield break;
      }

      while (this.DomainEvents.TryTake(out var domainEvent))
      {
        yield return domainEvent(serializer);
      }
    }

    /// <summary>
    /// Raises the domain event.
    /// </summary>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
    /// <param name="args">The <typeparamref name="TDomainEventArgs"/> instance containing the event data.</param>
    protected void RaiseDomainEvent<TDatabaseEntity, TDomainEventArgs>(TDomainEventArgs args)
      where TDatabaseEntity : DomainModelDatabaseEntity<TIdentifier, TModel>
      where TDomainEventArgs : DomainEventArgs<TModel>
      => this.DomainEvents.Add(serializer
        => DomainEventDatabaseEntity.Create<TDatabaseEntity, TIdentifier, TModel, TDomainEventArgs>(
          this.Created + this.Timer.Elapsed,
          this.Id,
          serializer.Serialize<TModel, TDomainEventArgs>(args)));
  }
}