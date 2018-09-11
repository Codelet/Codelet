namespace Codelet.Database.Entities
{
  /// <summary>
  /// The database entity with model.
  /// </summary>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public interface IDatabaseEntityWithModel<out TModel>
    where TModel : class
  {
    /// <summary>
    /// Gets the model.
    /// </summary>
    TModel Model { get; }
  }
}