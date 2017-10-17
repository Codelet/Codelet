namespace Codelet.Cqrs
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// An asynchronous operation.
  /// </summary>
  /// <typeparam name="TResult">The type of the operation result (must be <see cref="Task"/>).</typeparam>
  /// <typeparam name="TContext">The type of the operation execution context.</typeparam>
  public abstract class Operation<TResult, TContext>
    where TResult : Task
  {
    /// <summary>
    /// The operation context.
    /// </summary>
    protected class Context
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="Context"/> class.
      /// </summary>
      /// <param name="context">The context.</param>
      /// <param name="cancellationToken">The cancellation token.</param>
      /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="context" /> == <c>null</c>.</exception>
      public Context(TContext context, CancellationToken cancellationToken = default)
      {
        if (context == null)
        {
          throw new ArgumentNullException(nameof(context));
        }

        this.Services = context;
        this.CancellationToken = cancellationToken;
      }

      /// <summary>
      /// Gets the services.
      /// </summary>
      public TContext Services { get; }

      /// <summary>
      /// Gets the cancellation token.
      /// </summary>
      public CancellationToken CancellationToken { get; }
    }

    /// <summary>
    /// Executes this operation.
    /// </summary>
    /// <param name="context">The operation execution context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process and produces operation result.</returns>
    public virtual TResult ExecuteAsync(TContext context, CancellationToken cancellationToken = default(CancellationToken))
      => this.ExecuteAsync(new Context(context, cancellationToken));

    /// <summary>
    /// Executes this operation.
    /// </summary>
    /// <param name="context">The operation execution context.</param>
    /// <returns>The task that represents the process and produces operation result.</returns>
    protected abstract TResult ExecuteAsync(Context context);
  }
}
