﻿namespace Codelet.Cqrs
{
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// The command handler.
  /// </summary>
  /// <typeparam name="TCommand">The type of the command.</typeparam>
  public interface ICommandHandler<TCommand>
    where TCommand : struct, ICommand
  {
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    Task HandleAsync(in TCommand command, CancellationToken cancellationToken = default);
  }
}