namespace Codelet
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Extensions for <see cref="string"/>.
  /// </summary>
  public static class StringExtensions
  {
    /// <summary>
    /// Determines whether the specified <paramref name="value"/> has content.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <c>true</c> if the specified <paramref name="value"/> is not <c>null</c>, <see cref="string.Empty"/> or white space; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasContent(this string value)
      => !value.IsNullOrWhiteSpace();

    /// <summary>
    /// Determines whether the specified <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/> or white space.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <c>true</c> if the specified <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/> or white space; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNullOrWhiteSpace(this string value)
      => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Converts the specified <paramref name="value"/> to maybe structure.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// <see cref="Maybe{T}.None"/> if the specified <paramref name="value"/> is <c>null</c>, <see cref="string.Empty"/> or white space;
    /// otherwise, valuable <see cref="Maybe{T}"/>.
    /// </returns>
    public static Maybe<string> ToMaybe(this string value)
      => value;

    /// <summary>
    /// Joins the specified <paramref name="values"/> into the single string using the specified <paramref name="separator"/>.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>
    /// The result string.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="values"/> == <c>null</c>.</exception>
    public static string Join(this IEnumerable<string> values, string separator)
      => string.Join(separator, values);
  }
}