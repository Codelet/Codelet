namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// A query.
  /// </summary>
  /// <typeparam name="TResult">The type of the query result.</typeparam>
  /// <typeparam name="TContext">The type of the query execution context.</typeparam>
  public interface IQuery<TResult, in TContext>
  {
    /// <summary>
    /// Executes this query.
    /// </summary>
    /// <param name="context">The query execution context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process and produces query result.</returns>
    Task<TResult> ExecuteAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
  }
}
