namespace Codelet.Database.Domain
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Linq;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database context with domain events.
  /// </summary>
  public abstract class DatabaseContextWithDomainEvents : DatabaseContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContextWithDomainEvents" /> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="domainEventsSerializer">The domain events serializer.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="domainEventsSerializer" /> == <c>null</c>.</exception>
    protected DatabaseContextWithDomainEvents(
      DbContextOptions options,
      ILogger<DatabaseContextWithDomainEvents> logger,
      IDomainEventsSerializer domainEventsSerializer)
      : base(options, logger)
    {
      this.DomainEventsSerializer = domainEventsSerializer ?? throw new ArgumentNullException(nameof(domainEventsSerializer));
    }

    /// <summary>
    /// Gets the domain events.
    /// </summary>
    public DbSet<DomainEventDatabaseEntity> DomainEvents
      => this.Set<DomainEventDatabaseEntity>();

    /// <inheritdoc />
    protected override IEnumerable<Type> ConfigurationClasses
      => base
        .ConfigurationClasses
        .Concat(typeof(DomainEventDatabaseEntityConfiguration));

    private IDomainEventsSerializer DomainEventsSerializer { get; }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(
      bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default)
    {
      await this
        .TransferDomainEventsAsync(cancellationToken)
        .ConfigureAwait(false);

      var result = await base
        .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
        .ConfigureAwait(false);

      var domainEvents = await this
        .TransferDomainEventsAsync(cancellationToken)
        .ConfigureAwait(false);

      if (domainEvents.Count != 0)
      {
        result += await base
          .SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken)
          .ConfigureAwait(false);
      }

      foreach (var domainEvent in this.DomainEvents.Select(this.Entry))
      {
        domainEvent.State = EntityState.Detached;
      }

      return result;
    }

    private async Task<IReadOnlyCollection<DomainEventDatabaseEntity>> TransferDomainEventsAsync(
      CancellationToken cancellationToken)
    {
      var domainEvents = this
        .Entities<IDomainModelDatabaseEntity>()
        .SelectMany(entity => entity.TransferDomainEvents(this.DomainEventsSerializer))
        .ToArray();

      await this
        .DomainEvents
        .AddRangeAsync(domainEvents, cancellationToken)
        .ConfigureAwait(false);

      return domainEvents;
    }
  }
}