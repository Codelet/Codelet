namespace Codelet.Functional
{
  using System;

  /// <summary>
  /// LINQ extensions for <see cref="Maybe{T}"/>.
  /// </summary>
  public static class MaybeLinqExtensions
  {
    /// <summary>
    /// Projects the value of the <paramref name="maybe"/> (if exists) into a new form using <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="T">The type of the value to transform.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="maybe">The maybe.</param>
    /// <param name="selector">The transform function to apply to the maybe value.</param>
    /// <returns>
    /// The <see cref="Maybe{TResult}"/> whose value is the result of invoking the <paramref name="selector"/> on
    /// the value of the <paramref name="maybe"/> if it has value; otherwise, <see cref="Maybe{TResult}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="selector" /> == <c>null</c>.</exception>
    public static Maybe<TResult> Select<T, TResult>(
      this Maybe<T> maybe,
      Func<T, TResult> selector)
    {
      selector = selector ?? throw new ArgumentNullException(nameof(selector));

      return maybe.Select(value => (Maybe<TResult>)selector(value));
    }

    /// <summary>
    /// Projects the value of the <paramref name="maybe"/> (if exists) into a new form using <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="T">The type of the value to transform.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="maybe">The maybe.</param>
    /// <param name="selector">The transform function to apply to the maybe value.</param>
    /// <returns>
    /// The <see cref="Maybe{TResult}"/> whose value is the result of invoking the <paramref name="selector"/> on
    /// the value of the <paramref name="maybe"/> if it has value; otherwise, <see cref="Maybe{TResult}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="selector" /> == <c>null</c>.</exception>
    public static Maybe<TResult> Select<T, TResult>(
      this Maybe<T> maybe,
      Func<T, Maybe<TResult>> selector)
    {
      selector = selector ?? throw new ArgumentNullException(nameof(selector));

      return maybe.HasValue ? selector(maybe.Value) : Maybe<TResult>.None;
    }

    /// <summary>
    /// Projects the value of the given <paramref name="maybe"/> to another <see cref="Maybe{TInnerResult}"/>
    /// using <paramref name="innerSelector"/> and produces the combined result using <paramref name="outerSelector"/>.
    /// </summary>
    /// <typeparam name="T">The type of maybe value.</typeparam>
    /// <typeparam name="TInnerResult">The type of the inner result.</typeparam>
    /// <typeparam name="TOuterResult">The type of the outer result.</typeparam>
    /// <param name="maybe">The maybe.</param>
    /// <param name="innerSelector">The inner selector.</param>
    /// <param name="outerSelector">The outer selector.</param>
    /// <returns>
    /// The combined result produced by <paramref name="outerSelector"/>
    /// if both <paramref name="maybe"/> and projection returned by <paramref name="innerSelector"/> have values;
    /// otherwise, <see cref="Maybe{TOuterResult}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="innerSelector" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="outerSelector" /> == <c>null</c>.</exception>
    public static Maybe<TOuterResult> SelectMany<T, TInnerResult, TOuterResult>(
      this Maybe<T> maybe,
      Func<T, Maybe<TInnerResult>> innerSelector,
      Func<T, TInnerResult, TOuterResult> outerSelector)
    {
      innerSelector = innerSelector ?? throw new ArgumentNullException(nameof(innerSelector));
      outerSelector = outerSelector ?? throw new ArgumentNullException(nameof(outerSelector));

      return maybe.Select(value => innerSelector(value).Select(innerValue => outerSelector(value, innerValue)));
    }

    /// <summary>
    /// Filters the given <paramref name="maybe"/> value based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the maybe value.</typeparam>
    /// <param name="maybe">The maybe.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// <paramref name="maybe"/> if it has value and <paramref name="predicate"/> returns true;
    /// otherwise, <see cref="Maybe{T}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="predicate" /> == <c>null</c>.</exception>
    public static Maybe<T> Where<T>(this Maybe<T> maybe, Func<T, bool> predicate)
    {
      predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

      return maybe.Select(predicate).Select(value => value ? maybe : Maybe<T>.None);
    }
  }
}