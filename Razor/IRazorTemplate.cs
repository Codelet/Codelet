namespace Codelet.Razor
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Compiled razor template.
  /// </summary>
  public interface IRazorTemplate
  {
    /// <summary>
    /// Runs the template against the given <paramref name="model"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="model">The model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rendered text.</returns>
    Task<string> ExecuteAsync<TModel>(TModel model, CancellationToken cancellationToken = default);
  }
}