namespace Codelet.Database
{
  using System.Linq;

  /// <summary>
  /// The abstraction of database table.
  /// </summary>
  /// <typeparam name="TDatabaseEntity">The type of the database entity.</typeparam>
  public interface IDbSet<TDatabaseEntity>
    where TDatabaseEntity : class
  {
    /// <summary>
    /// Gets the interface to perform LINQ queries which will be translated into queries against the database.
    /// </summary>
    IQueryable<TDatabaseEntity> Query { get; }

    /// <summary>
    /// Gets an <see cref="IObservableReadOnlyCollection{TDatabaseEntity}" /> that represents
    /// a local view of all Added, Unchanged, and Modified entities in this set.
    /// This local view will stay in sync as entities are added or removed from the context.
    /// </summary>
    IObservableReadOnlyCollection<TDatabaseEntity> Local { get; }

    /// <summary>
    /// Adds the specified <paramref name="entity"/> to the database table.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Add(TDatabaseEntity entity);

    /// <summary>
    /// Removes the specified <paramref name="entity"/> from the database table.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void Remove(TDatabaseEntity entity);
  }
}