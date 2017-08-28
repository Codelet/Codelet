namespace Codelet
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Diagnostics.Contracts;

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

    /// <summary>
    /// Resolves maybe into <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will return <typeparamref name="T"/> constructed by <paramref name="valueFactory"/>.
    /// </summary>
    /// <param name="valueFactory">The value factory.</param>
    /// <returns>
    /// <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>; otherwise, the result of <paramref name="valueFactory"/> execution.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="valueFactory" /> == <c>null</c>.</exception>
    [Pure]
    public T Or(Func<T> valueFactory)
    {
      valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));

      return this.HasValue ? this.Value : valueFactory();
    }

    /// <summary>
    /// Resolves maybe into <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will throw the <typeparamref name="TException"/> constructed by <paramref name="exceptionFactory"/>.
    /// </summary>
    /// <typeparam name="TException">The type of the exception.</typeparam>
    /// <param name="exceptionFactory">The exception factory.</param>
    /// <returns>
    /// <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>; otherwise throws <typeparamref name="TException"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"> Thrown if <paramref name ="exceptionFactory" /> == <c>null</c>.</exception>
    [Pure]
    public T OrThrow<TException>(Func<TException> exceptionFactory)
      where TException : Exception
    {
      exceptionFactory = exceptionFactory ?? throw new ArgumentNullException(nameof(exceptionFactory));

      return this.Or(() => throw exceptionFactory());
    }

    /// <summary>
    /// Resolves maybe into <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>.
    /// If resolving is not possible it will return <c>default(T)</c>.
    /// </summary>
    /// <returns>
    /// <see cref="Value"/> if <see cref="HasValue"/> is <c>true</c>; otherwise, <c>default(T)</c>.
    /// </returns>
    [Pure]
    public T OrDefault()
      => this.Or(() => default(T));
  }
}