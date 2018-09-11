namespace Codelet.Domain
{
  using System;

  /// <summary>
  /// Provides the scope for the domain event arguments of <typeparamref name="TDomainEventArgs" /> type.
  /// </summary>
  /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
  /// <typeparam name="TDomainEventArgs">The type of the domain event arguments.</typeparam>
  public interface IDomainEventScopeProvider<in TDomainModel, in TDomainEventArgs>
    where TDomainModel : DomainModel
    where TDomainEventArgs : DomainEventArgs<TDomainModel>
  {
    /// <summary>
    /// Creates the scope for the given <paramref name="args"/>.
    /// </summary>
    /// <param name="args">The event arguments.</param>
    /// <returns>The disposable scope.</returns>
    IDisposable CreateScope(TDomainEventArgs args);
  }
}