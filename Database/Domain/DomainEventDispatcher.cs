namespace Codelet.Database.Domain
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Dispatches the domain event.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The task that represents the process.</returns>
  public delegate Task DomainEventDispatcher(CancellationToken cancellationToken);
}