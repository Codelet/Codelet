namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Extensions for <see cref="IEnumerable{T}"/> related to <see cref="Maybe{T}"/>.
  /// </summary>
  public static class EnumerableMaybeExtensions
  {
    /// <summary>
    /// Returns the element at a specified <paramref name="index" /> in a <paramref name="source" />
    /// or <see cref="Maybe{T}.None" /> if the index is out of range.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="index">The zero-based index of the element to retrieve.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None" /> if the <paramref name="index" /> is outside the bounds of the <paramref name="source" /> sequence;
    /// otherwise, the element at the specified position in the <paramref name="source" /> sequence.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    public static Maybe<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));

      return index < 0 ? Maybe<TSource>.None : source.Skip(index).FirstOrNone();
    }

    /// <summary>
    /// Returns the first element of the <paramref name="source" />,
    /// or <see cref="Maybe{T}.None" /> if the <paramref name="source" /> contains no elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None" /> if <paramref name="source" /> is empty; otherwise, the first element in <paramref name="source" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    public static Maybe<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source)
      => source.FirstOrNone(item => true);

    /// <summary>
    /// Returns the first element of the <paramref name="source" /> that satisfies a <paramref name="predicate" />,
    /// or <see cref="Maybe{T}.None" /> if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">The function to test each element for a condition.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None" /> if <paramref name="source" /> is empty
    /// or if no element passes the test specified <paramref name="predicate" />;
    /// otherwise, the first element in <paramref name="source" /> that passes the test specified <paramref name="predicate" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate" /> == <c>null</c>.</exception>
    public static Maybe<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));
      predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

      return source
        .Where(predicate)
        .Select(item => (Maybe<TSource>)item)
        .FirstOrDefault();
    }

    /// <summary>
    /// Returns the last element of the <paramref name="source" />,
    /// or <see cref="Maybe{T}.None" />if the <paramref name="source" /> contains no elements.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None" /> if the <paramref name="source" /> is empty;
    /// otherwise, the last element in the <paramref name="source" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    public static Maybe<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source)
      => source.LastOrNone(item => true);

    /// <summary>
    /// Returns the last element of the <paramref name="source" /> that satisfies <paramref name="predicate"/>
    /// or <see cref="Maybe{T}.None" /> if no such element is found.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None" /> if the <paramref name="source" /> is empty
    /// or if no elements pass the test in the <paramref name="predicate"/> function;
    /// otherwise, the last element that passes the test in the <paramref name="predicate"/> function.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate" /> == <c>null</c>.</exception>
    public static Maybe<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));
      predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

      return source
        .Where(predicate)
        .Select(item => (Maybe<TSource>)item)
        .LastOrDefault();
    }

    /// <summary>
    /// Returns the only element of the <paramref name="source" />,
    /// or <see cref="Maybe{T}.None" />if the <paramref name="source" /> contains no elements;
    /// this method throws an exception if there is more than one element in the <paramref name="source" />.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>
    /// The single element of the <paramref name="source" /> or <see cref="Maybe{T}.None" /> if the <paramref name="source" /> is empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    public static Maybe<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source)
      => source.SingleOrNone(item => true);

    /// <summary>
    /// Returns the only element of the <paramref name="source" /> that satisfies <paramref name="predicate"/>
    /// or <see cref="Maybe{T}.None" /> if no such element is found;
    /// this method throws an exception if more than one element satisfies the <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// The single element of the <paramref name="source" />that satisfies <paramref name="predicate"/>,
    /// or <see cref="Maybe{T}.None" /> if no elements found.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate" /> == <c>null</c>.</exception>
    public static Maybe<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));
      predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

      return source
        .Where(predicate)
        .Select(item => (Maybe<TSource>)item)
        .SingleOrDefault();
    }
  }
}