namespace Codelet.Domain
{
  using System;

  /// <summary>
  /// Provides the scope for the domain models of <typeparamref name="TDomainModel"/> type.
  /// </summary>
  /// <typeparam name="TDomainModel">The type of the domain model.</typeparam>
  public interface IDomainModelScopeProvider<in TDomainModel>
    where TDomainModel : DomainModel
  {
    /// <summary>
    /// Creates the scope for the given <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The disposable scope.</returns>
    IDisposable CreateScope(TDomainModel model);
  }
}