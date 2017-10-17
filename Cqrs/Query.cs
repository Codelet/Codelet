namespace Codelet.Cqrs
{
  using System.Threading.Tasks;

  /// <summary>
  /// A query.
  /// </summary>
  /// <typeparam name="TResult">The type of the query result.</typeparam>
  /// <typeparam name="TContext">The type of the query execution context.</typeparam>
  public abstract class Query<TResult, TContext>
    : Operation<Task<TResult>, TContext>
  {
  }
}
