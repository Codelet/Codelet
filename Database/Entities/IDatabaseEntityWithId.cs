namespace Codelet.Database.Entities
{
  /// <summary>
  /// The database entity with database identifier.
  /// </summary>
  /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
  public interface IDatabaseEntityWithId<out TIdentifier>
  {
    /// <summary>
    /// Gets the identifier.
    /// </summary>
    TIdentifier Id { get; }
  }
}