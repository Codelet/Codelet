namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// A command.
  /// </summary>
  /// <typeparam name="TContext">The type of the command execution context.</typeparam>
  public interface ICommand<in TContext>
  {
    /// <summary>
    /// Executes this query.
    /// </summary>
    /// <param name="context">The command execution context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    Task ExecuteAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken));
  }
}
