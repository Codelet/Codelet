namespace Codelet.Database.Entities
{
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// The database entity.
  /// </summary>
  public interface IDatabaseEntity
  {
    /// <summary>
    /// Synchronizes this database entity to have all actual values.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// Usually is called before commit to make sure database receives the latest data.
    /// </remarks>
    void Synchronize(ILogger logger);
  }
}