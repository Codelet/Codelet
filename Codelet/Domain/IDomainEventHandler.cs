namespace Codelet.Domain
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Handles the domain events of type <typeparamref name="TDomainEventArgs"/> for the models of type <typeparamref name="TDomainModel"/>.
  /// </summary>
  /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
  /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
  public interface IDomainEventHandler<in TDomainModel, in TDomainEventArgs>
    where TDomainModel : DomainModel
    where TDomainEventArgs : DomainEventArgs<TDomainModel>
  {
    /// <summary>
    /// Handles the domain event with the given <paramref name="args"/> for the specified <paramref name="sender"/>.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <typeparamref name="TDomainEventArgs"/> instance containing the event data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    Task HandleAsync(
      TDomainModel sender,
      TDomainEventArgs args,
      CancellationToken cancellationToken);
  }
}