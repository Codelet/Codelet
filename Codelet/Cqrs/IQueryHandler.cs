namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// The query handler.
  /// </summary>
  /// <typeparam name="TQuery">The type of the query.</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  public interface IQueryHandler<in TQuery, TResult>
  {
    /// <summary>
    /// Executes the query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query result.</returns>
    Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
  }
}
