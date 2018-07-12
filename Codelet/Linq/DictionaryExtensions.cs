namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading.Tasks;

  /// <summary>
  /// Extension methods for <see cref="IDictionary{TKey, TValue}"/>.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    /// Gets the value associated with the specified key.
    /// If <paramref name="dictionary"/> contains no value associated with the give <paramref name="key"/>,
    /// <paramref name="factory"/> is used to create the value and put it into the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="dictionary" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="factory" /> == <c>null</c>.</exception>
    public static TValue GetOrCreate<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TValue> factory)
    {
      dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
      factory = factory ?? throw new ArgumentNullException(nameof(factory));

      if (!dictionary.TryGetValue(key, out var value))
      {
        value = factory();
        dictionary[key] = value;
      }

      return value;
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// If <paramref name="dictionary"/> contains no value associated with the give <paramref name="key"/>,
    /// <paramref name="factory"/> is used to create the value and put it into the <paramref name="dictionary"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key.</param>
    /// <param name="factory">The factory.</param>
    /// <returns>The value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="dictionary" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="factory" /> == <c>null</c>.</exception>
    public static async Task<TValue> GetOrCreateAsync<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<Task<TValue>> factory)
    {
      dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
      factory = factory ?? throw new ArgumentNullException(nameof(factory));

      if (!dictionary.TryGetValue(key, out var value))
      {
        value = await factory().ConfigureAwait(false);
        dictionary[key] = value;
      }

      return value;
    }

    /// <summary>
    /// Wraps the dictionary with read-only adapter.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <returns>The read-only dictionary adapter.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="dictionary" /> == <c>null</c>.</exception>
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary)
      => new ReadOnlyDictionary<TKey, TValue>(
        dictionary ?? throw new ArgumentNullException(nameof(dictionary)));

    /// <summary>
    /// Converts the specified <paramref name="source"/> to dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> source)
      => (source ?? throw new ArgumentNullException(nameof(source)))
        .ToDictionary(e => e.Key, e => e.Value);

    /// <summary>
    /// Converts the specified <paramref name="source"/> to dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="comparer" /> == <c>null</c>.</exception>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
      this IEnumerable<KeyValuePair<TKey, TValue>> source,
      IEqualityComparer<TKey> comparer)
      => (source ?? throw new ArgumentNullException(nameof(source)))
        .ToDictionary(e => e.Key, e => e.Value, comparer ?? throw new ArgumentNullException(nameof(comparer)));

    /// <summary>
    /// Converts the specified <paramref name="source"/> to dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
      this IEnumerable<ValueTuple<TKey, TValue>> source)
      => (source ?? throw new ArgumentNullException(nameof(source)))
        .ToDictionary(e => e.Item1, e => e.Item2);

    /// <summary>
    /// Converts the specified <paramref name="source"/> to dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="source" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="comparer" /> == <c>null</c>.</exception>
    public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(
      this IEnumerable<ValueTuple<TKey, TValue>> source,
      IEqualityComparer<TKey> comparer)
      => (source ?? throw new ArgumentNullException(nameof(source)))
        .ToDictionary(e => e.Item1, e => e.Item2, comparer ?? throw new ArgumentNullException(nameof(comparer)));
  }
}