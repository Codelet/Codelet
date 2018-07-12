namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// The command handler.
  /// </summary>
  /// <typeparam name="TCommand">The type of the command.</typeparam>
  public interface ICommandHandler<in TCommand>
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
  }
}