namespace Codelet
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Various generic extension.
  /// </summary>
  public static class GenericExtensions
  {
    /// <summary>
    /// Flattens the specified <paramref name="target"/> using the <paramref name="flatten"/> delegate to traverse.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="target">The target.</param>
    /// <param name="flatten">The traverse delegate applied to each node.</param>
    /// <returns>
    /// The flat sequence of elements.
    /// </returns>
    public static IEnumerable<TSource> Flatten<TSource>(this TSource target, Func<TSource, TSource> flatten)
    {
      while (target != null)
      {
        yield return target;
        target = flatten(target);
      }
    }

    /// <summary>
    /// Flattens the specified <paramref name="target"/> using the <paramref name="flatten"/> delegate to traverse.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="target">The target.</param>
    /// <param name="flatten">The traverse delegate applied to each node.</param>
    /// <returns>
    /// The flat sequence of elements.
    /// </returns>
    public static IEnumerable<TSource> Flatten<TSource>(this TSource target, Func<TSource, IEnumerable<TSource>> flatten)
      => target != null
      ? Enumerable.Repeat(target, 1).Concat(flatten(target).Where(t => t != null).SelectMany(t => t.Flatten(flatten)))
      : Enumerable.Empty<TSource>();
  }
}