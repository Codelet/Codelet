namespace Codelet.Database.Entities
{
  using System.Linq;

  /// <summary>
  /// The database entity configuration
  /// </summary>
  /// <typeparam name="TEntity">The type of the entity.</typeparam>
  public interface IDatabaseEntityConfiguration<TEntity>
    where TEntity : class
  {
    /// <summary>
    /// Configures the includes for this entity type.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The configured query.</returns>
    IQueryable<TEntity> ConfigureIncludes(IQueryable<TEntity> entity);
  }
}