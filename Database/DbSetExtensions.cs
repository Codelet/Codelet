namespace Codelet.Database
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Codelet.Database.Entities;
  using Codelet.Linq;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Extends <see cref="IDbSet{TDatabaseEntity}"/> with common methods.
  /// </summary>
  public static class DbSetExtensions
  {
    /// <summary>
    /// Finds the entity by identifier.
    /// </summary>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TDatabaseEntityId">The type of the database entity identifier.</typeparam>
    /// <param name="table">The table.</param>
    /// <param name="id">The identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The found entity.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="table" /> == <c>null</c>.</exception>
    public static async Task<Maybe<TDatabaseEntity>> FindEntityByIdAsync<TDatabaseEntity, TDatabaseEntityId>(
      this IDbSet<TDatabaseEntity> table,
      TDatabaseEntityId id,
      CancellationToken cancellationToken = default)
      where TDatabaseEntity : class, IDatabaseEntityWithId<TDatabaseEntityId>
    {
      table = table ?? throw new ArgumentNullException(nameof(table));

      return table.Local.FirstOrDefault(e => e.Id.Equals(id))
        ?? await table
          .Query
          .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken)
          .ConfigureAwait(false);
    }

    /// <summary>
    /// Finds the entity by model.
    /// </summary>
    /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="table">The table.</param>
    /// <param name="model">The model.</param>
    /// <returns>
    /// The found entity.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="table" /> == <c>null</c>.</exception>
    public static Maybe<TDatabaseEntity> FindEntityByModel<TDatabaseEntity, TModel>(
      this IDbSet<TDatabaseEntity> table,
      TModel model)
      where TDatabaseEntity : class, IDatabaseEntityWithModel<TModel>
      where TModel : class
    {
      table = table ?? throw new ArgumentNullException(nameof(table));

      return table.Local.FirstOrNone(entity => entity.Model == model);
    }
  }
}