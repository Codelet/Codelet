namespace Codelet.Database.Entities
{
  /// <summary>
  /// The database entity.
  /// </summary>
  public interface IDatabaseEntity
  {
    /// <summary>
    /// Synchronizes this database entity to have all actual values.
    /// </summary>
    /// <remarks>
    /// Usually is called before commit to make sure database receives the latest data.
    /// </remarks>
    void Synchronize();
  }
}