namespace Codelet
{
  /// <summary>
  /// Extension methods for the <see cref="Maybe{T}"/>.
  /// </summary>
  public static class MaybeExtensions
  {
    /// <summary>
    /// Resolves <paramref name="maybe"/> into <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will return <c>true</c>.
    /// </summary>
    /// <param name="maybe">The maybe instance.</param>
    /// <returns>
    /// <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>; otherwise, <c>true</c>.
    /// </returns>
    public static bool OrTrue(this Maybe<bool> maybe)
      => maybe.Or(() => true);

    /// <summary>
    /// Resolves <paramref name="maybe"/> into <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will return <c>false</c>.
    /// </summary>
    /// <param name="maybe">The maybe instance.</param>
    /// <returns>
    /// <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>; otherwise, <c>false</c>.
    /// </returns>
    public static bool OrFalse(this Maybe<bool> maybe)
      => maybe.Or(() => false);

    /// <summary>
    /// Resolves <paramref name="maybe"/> into <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will return <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="maybe">The maybe instance.</param>
    /// <returns>
    /// <see cref="Maybe{T}.Value"/> if <see cref="Maybe{T}.HasValue"/> is <c>true</c>; otherwise, <see cref="string.Empty"/>.
    /// </returns>
    public static string OrEmpty(this Maybe<string> maybe)
      => maybe.Or(() => string.Empty);

    /// <summary>
    /// Flattens nested maybe into non-nested one.
    /// </summary>
    /// <typeparam name="T">The type of the maybe internal value.</typeparam>
    /// <param name="maybe">The maybe instance.</param>
    /// <returns>
    /// The non-nested <see cref="Maybe{T}"/>.
    /// </returns>
    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> maybe)
      => maybe.HasValue && maybe.Value.HasValue ? maybe.Value.Value : Maybe<T>.None;
  }
}