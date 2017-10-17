namespace Codelet.Cqrs
{
  using System.Threading.Tasks;

  /// <summary>
  /// A command.
  /// </summary>
  /// <typeparam name="TContext">The type of the command execution context.</typeparam>
  public abstract class Command<TContext>
    : Operation<Task, TContext>
  {
  }
}
