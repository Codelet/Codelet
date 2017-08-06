namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Extensions for <see cref="IDictionary{TKey, TValue}"/> related to <see cref="Maybe{T}"/>.
  /// </summary>
  public static class DictionaryMaybeExtensions
  {
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary.</param>
    /// <param name="key">The key whose value to get.</param>
    /// <returns>
    /// The corresponding value if the <paramref name="dictionary"/> contains an element with the specified <paramref name="key"/>;
    /// otherwise, <see cref="Maybe{TValue}.None"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="dictionary" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="key" /> == <c>null</c>.</exception>
    public static Maybe<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
      => (dictionary ?? throw new ArgumentNullException(nameof(dictionary))).TryGetValue(key, out TValue controllerType)
      ? controllerType
      : Maybe<TValue>.None;
  }
}