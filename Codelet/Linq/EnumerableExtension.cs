namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Extensions for <see cref="IEnumerable{T}"/>.
  /// </summary>
  public static class EnumerableExtension
  {
    /// <summary>
    /// Adds the specified <paramref name="items"/> to the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="collection" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="items" /> == <c>null</c>.</exception>
    public static void AddRange<T>(this ICollection<T> collection, params T[] items)
    {
      collection = collection ?? throw new ArgumentNullException(nameof(collection));
      items = items ?? throw new ArgumentNullException(nameof(items));

      collection.AddRange((IEnumerable<T>)items);
    }

    /// <summary>
    /// Adds the specified <paramref name="items"/> to the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="collection" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="items" /> == <c>null</c>.</exception>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      collection = collection ?? throw new ArgumentNullException(nameof(collection));
      items = items ?? throw new ArgumentNullException(nameof(items));

      items.ForEach(collection.Add);
    }

    /// <summary>
    /// Removes the specified <paramref name="items"/> from the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="collection" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="items" /> == <c>null</c>.</exception>
    public static void RemoveAll<T>(this ICollection<T> collection, params T[] items)
    {
      collection = collection ?? throw new ArgumentNullException(nameof(collection));
      items = items ?? throw new ArgumentNullException(nameof(items));

      collection.RemoveAll((IEnumerable<T>)items);
    }

    /// <summary>
    /// Removes the specified <paramref name="items"/> from the <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="items">The items.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="collection" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="items" /> == <c>null</c>.</exception>
    public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
      collection = collection ?? throw new ArgumentNullException(nameof(collection));
      items = items ?? throw new ArgumentNullException(nameof(items));

      items.ForEach(item => collection.Remove(item));
    }

    /// <summary>
    /// Removes all the elements that match the conditions defined by the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    /// <param name="collection">The collection.</param>
    /// <param name="predicate">The delegate that defines the conditions of the elements to remove.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="collection" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="predicate" /> == <c>null</c>.</exception>
    public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
      collection = collection ?? throw new ArgumentNullException(nameof(collection));
      predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

      collection.Where(predicate).ToArray().ForEach(item => collection.Remove(item));
    }

    /// <summary>
    /// Concatenates the specified <paramref name="source"/> with the specified <paramref name="item"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The sequence to concatenate.</param>
    /// <param name="item">The item to concatenate.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains the concatenated elements.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
      => (source ?? throw new ArgumentNullException(nameof(source))).Concat(new[] { item });

    /// <summary>
    ///  Returns distinct elements from a <paramref name="source"/> by comparing the projected values.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <typeparam name="TProjection">The type of the projected value.</typeparam>
    /// <param name="source">The sequence of items.</param>
    /// <param name="selector">The function that projects the each element of the <paramref name="source"/>.</param>
    /// <returns>
    /// The <see cref="IEnumerable{T}"/> that contains distinct elements from <paramref name="source"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="selector" /> == <c>null</c>.</exception>
    public static IEnumerable<T> Distinct<T, TProjection>(this IEnumerable<T> source, Func<T, TProjection> selector)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));
      selector = selector ?? throw new ArgumentNullException(nameof(selector));

      var projectionsComparer = EqualityComparer<TProjection>.Default;
      var comparer = new RelayEqualityComparer<T>(
        obj => selector(obj).GetHashCode(),
        (lhs, rhs) => projectionsComparer.Equals(selector(lhs), selector(rhs)));

      return source.Distinct(comparer);
    }

    /// <summary>
    /// Determines whether a <paramref name="source"/> contains no elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The sequence of items.</param>
    /// <returns><c>true</c> if the <paramref name="source"/> contains no elements; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    public static bool None<T>(this IEnumerable<T> source)
      => !source.Any();

    /// <summary>
    /// Determines whether any element of a <paramref name="source"/> satisfies a condition.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The sequence of items.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>
    /// <c>false</c> if any element in the <paramref name="source"/> pass the test in the specified <paramref name="predicate"/>;
    /// otherwise, <c>true</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="predicate" /> == <c>null</c>.</exception>
    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
      => !source.Any(predicate);

    /// <summary>
    /// Invokes the specified <paramref name="action"/> for each element of the <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the input sequence.</typeparam>
    /// <param name="source">The sequence of items.</param>
    /// <param name="action">The action.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="action" /> == <c>null</c>.</exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
      source = source ?? throw new ArgumentNullException(nameof(source));
      action = action ?? throw new ArgumentNullException(nameof(action));

      foreach (var item in source)
      {
        action(item);
      }
    }
  }
}