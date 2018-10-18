namespace Codelet.Database
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Domain;

  /// <summary>
  /// The basic repository interface.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of the domain model persistent identifier.</typeparam>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public interface IRepository<TIdentifier, TModel>
    where TModel : DomainModel
  {
    /// <summary>
    /// Adds the specified model to the repository.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model" /> == <c>null</c>.</exception>
    void Add(TModel model);

    /// <summary>
    /// Removes the specified model from the repository.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="model" /> == <c>null</c>.</exception>
    void Remove(TModel model);

    /// <summary>
    /// Gets the identifier for the given <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The persistent identifier.</returns>
    Maybe<TIdentifier> GetId(TModel model);

    /// <summary>
    /// Finds the model by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found model.</returns>
    Task<Maybe<TModel>> FindByIdAsync(TIdentifier id, CancellationToken cancellationToken = default);
  }
}