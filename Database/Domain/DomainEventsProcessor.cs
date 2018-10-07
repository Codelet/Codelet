namespace Codelet.Database.Domain
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Processes domain events on-by-one.
  /// </summary>
  public class DomainEventsProcessor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventsProcessor" /> class.
    /// </summary>
    /// <param name="database">The database.</param>
    /// <param name="factory">The factory.</param>
    /// <param name="exceptionHandler">The exception handler.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="database" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="exceptionHandler" /> == <c>null</c>.</exception>
    public DomainEventsProcessor(
      DatabaseContextWithDomainEvents database,
      IDomainEventsDispatcherFactory factory,
      Func<Exception, Task> exceptionHandler)
    {
      this.Database = database ?? throw new ArgumentNullException(nameof(database));
      this.Query = this.Database.DomainEvents.OrderBy(e => e.Occured);
      this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
      this.ExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
    }

    private DatabaseContextWithDomainEvents Database { get; }

    private IQueryable<DomainEventDatabaseEntity> Query { get; }

    private IDomainEventsDispatcherFactory Factory { get; }

    private Func<Exception, Task> ExceptionHandler { get; }

    private CancellationTokenSource CancellationTokenSource { get; set; }

    private Task Processor { get; set; }

    /// <summary>
    /// Starts the domain events processing on a separate thread.
    /// </summary>
    public void Start()
    {
      if (this.Processor != null)
      {
        return;
      }

      this.CancellationTokenSource = new CancellationTokenSource();
      this.Processor = Task.Run(() => this.ProcessMessagesAsync(this.CancellationTokenSource.Token));
    }

    /// <summary>
    /// Stops the domain events processing.
    /// </summary>
    /// <returns>The task that represents the process.</returns>
    public async Task StopAsync()
    {
      this.CancellationTokenSource.Cancel();

      try
      {
        await this.Processor.ConfigureAwait(false);
      }
      catch (TaskCanceledException)
      {
      }

      this.CancellationTokenSource = null;
      this.Processor = null;
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
      async Task<DomainEventDatabaseEntity> GetEventAsync()
      {
        while (true)
        {
          cancellationToken.ThrowIfCancellationRequested();

          var oldestEvent = await this
            .Query
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

          if (oldestEvent != null)
          {
            return oldestEvent;
          }

          await Task
            .Delay(TimeSpan.FromSeconds(3), cancellationToken)
            .ConfigureAwait(false);
        }
      }

      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          var oldestEvent = await GetEventAsync().ConfigureAwait(false);

          var dispatcher = oldestEvent.Deserialize(this.Factory);

          await dispatcher(cancellationToken).ConfigureAwait(false);

          this.Database.DomainEvents.Remove(oldestEvent);

          await this.Database
            .CommitAsync(cancellationToken)
            .ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception exception)
        {
          await this.ExceptionHandler(exception).ConfigureAwait(false);
        }
      }
    }
  }
}