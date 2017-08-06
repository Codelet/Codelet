namespace Codelet
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  /// <summary>
  /// Defines an optional value of type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The type of the value.</typeparam>
  [DebuggerDisplay("{HasValue ? Value.ToString() : \"None\"}")]
  public struct Maybe<T> : IEquatable<Maybe<T>>
  {
    /// <summary>
    /// The instance that contains no value.
    /// </summary>
    public static readonly Maybe<T> None = default(Maybe<T>);

    private readonly T value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Maybe{T}"/> struct.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> == <c>null</c>.</exception>
    public Maybe(T value)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }

      this.HasValue = true;
      this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this instance has value.
    /// </summary>
    public bool HasValue { get; }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>false</c>.</exception>
    public T Value
      => this.HasValue ? this.value : throw new InvalidOperationException("Maybe structure has no value.");

    /// <summary>
    /// Performs an implicit conversion from <typeparamref name="T"/> to <see cref="Maybe{T}"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator Maybe<T>(T value)
      => value != null && ((value as string)?.HasContent() ?? true) ? new Maybe<T>(value) : None;

    /// <summary>
    /// Implements the operator ==.
    /// </summary>
    /// <param name="lhs">The left-hand side.</param>
    /// <param name="rhs">The right-hand side.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="lhs"/> equals <paramref name="rhs"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Maybe<T> lhs, Maybe<T> rhs)
      => Equals(lhs, rhs);

    /// <summary>
    /// Implements the operator !=.
    /// </summary>
    /// <param name="lhs">The left-hand side.</param>
    /// <param name="rhs">The right-hand side.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="lhs"/> does not equal <paramref name="rhs"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Maybe<T> lhs, Maybe<T> rhs)
      => !(lhs == rhs);

    /// <summary>
    /// Check equality of the specified slices.
    /// </summary>
    /// <param name="lhs">The left-hand side.</param>
    /// <param name="rhs">The right-hand side.</param>
    /// <returns>
    /// <c>true</c> if the specified instances are equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool Equals(Maybe<T> lhs, Maybe<T> rhs)
      => (!lhs.HasValue && !rhs.HasValue)
      || (lhs.HasValue && rhs.HasValue && EqualityComparer<T>.Default.Equals(lhs.Value, rhs.Value));

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
      => obj is Maybe<T> && this.Equals((Maybe<T>)obj);

    /// <summary>
    /// Determines whether the specified <paramref name="other"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The other instance to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <paramref name="other"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Maybe<T> other)
      => Equals(this, other);

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
      => this.HasValue ? this.Value.GetHashCode() : 0;
  }
}