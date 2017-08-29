namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Handles the commands of type <typeparamref name="TCommand"/>.
  /// </summary>
  /// <typeparam name="TCommand">The type of the command.</typeparam>
  public interface ICommandHandler<in TCommand>
    where TCommand : Command
  {
    /// <summary>
    /// Executes the given <paramref name="command"/>.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    Task ExecuteAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
  }
}