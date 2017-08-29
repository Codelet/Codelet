namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Handles the commands of type <typeparamref name="TQuery"/>.
  /// </summary>
  /// <typeparam name="TQuery">The type of the query.</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  public interface IQueryHandler<in TQuery, TResult>
    where TQuery : Query<TResult>
  {
    /// <summary>
    /// Executes the given <paramref name="query"/>.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The query result.</returns>
    Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default(CancellationToken));
  }
}