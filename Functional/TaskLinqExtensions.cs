namespace Codelet.Functional
{
  using System;
  using System.Threading.Tasks;

  /// <summary>
  /// LINQ extensions for <see cref="Task{T}"/>.
  /// </summary>
  public static class TaskLinqExtensions
  {
    /// <summary>
    /// Projects the result of the <paramref name="task"/> into a new form using <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="T">The type of the value to transform.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="selector">The transform function to apply to the task result.</param>
    /// <returns>
    /// The <see cref="Task{TResult}"/> whose value is the result of invoking the <paramref name="selector"/> on
    /// the result of the <paramref name="task"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="task" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="selector" /> == <c>null</c>.</exception>
    public static async Task<TResult> Select<T, TResult>(
      this Task<T> task,
      Func<T, TResult> selector)
    {
      task = task ?? throw new ArgumentNullException(nameof(task));
      selector = selector ?? throw new ArgumentNullException(nameof(selector));

      return selector(await task.ConfigureAwait(false));
    }

    /// <summary>
    /// Projects the result of the <paramref name="task"/> into a new form using <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="T">The type of the value to transform.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="selector">The transform function to apply to the task result.</param>
    /// <returns>
    /// The <see cref="Task{TResult}"/> whose value is the result of invoking the <paramref name="selector"/> on
    /// the result of the <paramref name="task"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="task" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="selector" /> == <c>null</c>.</exception>
    public static Task<TResult> Select<T, TResult>(
      this Task<T> task,
      Func<T, Task<TResult>> selector)
    {
      task = task ?? throw new ArgumentNullException(nameof(task));
      selector = selector ?? throw new ArgumentNullException(nameof(selector));

      return task.Select<T, Task<TResult>>(selector).Unwrap();
    }

    /// <summary>
    /// Projects the result of the given <paramref name="task"/> to another <see cref="Task{TInnerResult}"/>
    /// using <paramref name="innerSelector"/> and produces the combined result using <paramref name="outerSelector"/>.
    /// </summary>
    /// <typeparam name="T">The type of maybe value.</typeparam>
    /// <typeparam name="TInnerResult">The type of the inner result.</typeparam>
    /// <typeparam name="TOuterResult">The type of the outer result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="innerSelector">The inner selector.</param>
    /// <param name="outerSelector">The outer selector.</param>
    /// <returns>
    /// The task that returns combined result produced by <paramref name="outerSelector"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="task" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="innerSelector" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="outerSelector" /> == <c>null</c>.</exception>
    public static Task<TOuterResult> SelectMany<T, TInnerResult, TOuterResult>(
      this Task<T> task,
      Func<T, Task<TInnerResult>> innerSelector,
      Func<T, TInnerResult, TOuterResult> outerSelector)
    {
      task = task ?? throw new ArgumentNullException(nameof(task));
      innerSelector = innerSelector ?? throw new ArgumentNullException(nameof(innerSelector));
      outerSelector = outerSelector ?? throw new ArgumentNullException(nameof(outerSelector));

      return task.Select(value => innerSelector(value).Select(innerValue => outerSelector(value, innerValue)));
    }

    /// <summary>
    /// Projects the result of the given <paramref name="task"/> to another <see cref="Maybe{TInnerResult}"/>
    /// using <paramref name="innerSelector"/> and produces the combined result using <paramref name="outerSelector"/>.
    /// </summary>
    /// <typeparam name="T">The type of maybe value.</typeparam>
    /// <typeparam name="TInnerResult">The type of the inner result.</typeparam>
    /// <typeparam name="TOuterResult">The type of the outer result.</typeparam>
    /// <param name="task">The task.</param>
    /// <param name="innerSelector">The inner selector.</param>
    /// <param name="outerSelector">The outer selector.</param>
    /// <returns>
    /// The task that returns combined result produced by <paramref name="outerSelector"/>
    /// if projection returned by <paramref name="innerSelector"/> has value;
    /// otherwise, the task that returns <see cref="Maybe{TOuterResult}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="task" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="innerSelector" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="outerSelector" /> == <c>null</c>.</exception>
    public static Task<Maybe<TOuterResult>> SelectMany<T, TInnerResult, TOuterResult>(
      this Task<T> task,
      Func<T, Maybe<TInnerResult>> innerSelector,
      Func<T, TInnerResult, TOuterResult> outerSelector)
    {
      task = task ?? throw new ArgumentNullException(nameof(task));
      innerSelector = innerSelector ?? throw new ArgumentNullException(nameof(innerSelector));
      outerSelector = outerSelector ?? throw new ArgumentNullException(nameof(outerSelector));

      return task.Select(value => innerSelector(value).Select(innerValue => outerSelector(value, innerValue)));
    }
  }
}
