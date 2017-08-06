namespace Codelet
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Implements of <see cref="IEqualityComparer{T}"/> using external delegate to perform items comparison.
  /// </summary>
  /// <typeparam name="T">The type of the items to compare.</typeparam>
  public struct RelayEqualityComparer<T> : IEqualityComparer<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RelayEqualityComparer{T}" /> struct.
    /// </summary>
    /// <param name="hasher">The hasher.</param>
    /// <param name="comparer">The comparer.</param>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="hasher" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="comparer" /> == <c>null</c>.</exception>
    public RelayEqualityComparer(Func<T, int> hasher, Func<T, T, bool> comparer)
    {
      this.Hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
      this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    private Func<T, T, bool> Comparer { get; }

    private Func<T, int> Hasher { get; }

    /// <inheritdoc />
    public bool Equals(T x, T y)
      => this.Comparer(x, y);

    /// <inheritdoc />
    public int GetHashCode(T obj)
      => this.Hasher(obj);
  }
}