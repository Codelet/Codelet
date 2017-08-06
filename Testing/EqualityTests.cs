namespace Codelet.Testing
{
  using System;
  using System.Linq.Expressions;
  using FluentAssertions;
  using Xunit;

  /// <summary>
  /// Base class for equality tests.
  /// </summary>
  /// <typeparam name="TSubject">The type of the subject.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  public abstract class EqualityTests<TSubject, TValue>
    where TSubject : IEquatable<TValue>
  {
    private const string MustBeEqualReason = "because subject must be equal to equal value";
    private const string MustBeInequalReason = "because subject must not be equal to unequal value";

    /// <summary>
    /// Initializes a new instance of the <see cref="EqualityTests{TSubject, TValue}"/> class.
    /// </summary>
    /// <param name="subject">The subject.</param>
    /// <param name="equalValue">The equal value (must be equal to <paramref name="subject"/>).</param>
    /// <param name="unequalValue">The unequal value (must be unequal to <paramref name="subject"/>).</param>
    protected EqualityTests(TSubject subject, TValue equalValue, TValue unequalValue)
    {
      this.Subject = subject;
      this.EqualValue = equalValue;
      this.UnequalValue = unequalValue;
    }

    private TSubject Subject { get; }

    private TValue EqualValue { get; }

    private TValue UnequalValue { get; }

    /// <summary>
    /// Checks that <see cref="IEquatable{T}.Equals(T)"/> returns <c>true</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void EqualsReturnsTrueForEqualValue()
      => this.Subject.Equals(this.EqualValue).Should().BeTrue(MustBeEqualReason);

    /// <summary>
    /// Checks that <see cref="object.Equals(object)"/> returns <c>true</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void ObjectEqualsReturnsTrueForEqualValue()
    {
      ((object)this.Subject).Equals(this.EqualValue).Should().BeTrue(MustBeEqualReason);
      this.Subject.Should().Be(this.EqualValue, MustBeEqualReason);
    }

    /// <summary>
    /// Checks that equality operator returns <c>true</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void EqualityOperatorReturnsTrueForEqualValue()
    {
      ApplyExpression(Expression.Equal, this.Subject, this.EqualValue).Should().BeTrue(MustBeEqualReason);
      ApplyExpression(Expression.Equal, this.EqualValue, this.Subject).Should().BeTrue(MustBeEqualReason);
    }

    /// <summary>
    /// Checks that inequality operator returns <c>false</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void InequalityOperatorReturnsFalseForEqualValue()
    {
      ApplyExpression(Expression.NotEqual, this.Subject, this.EqualValue).Should().BeFalse(MustBeEqualReason);
      ApplyExpression(Expression.NotEqual, this.EqualValue, this.Subject).Should().BeFalse(MustBeEqualReason);
    }

    /// <summary>
    /// Checks that <see cref="object.GetHashCode()"/> returns the same value for subject and equal value.
    /// </summary>
    [Fact]
    public void GetHasCodeReturnsTheSameHashForEqualValue()
      => this.Subject.GetHashCode().Should().Be(this.EqualValue.GetHashCode(), MustBeEqualReason);

    /// <summary>
    /// Checks that <see cref="IEquatable{T}.Equals(T)"/> returns <c>false</c> for subject and unequal value.
    /// </summary>
    [Fact]
    public void EqualsReturnsFalseForUnequalValue()
      => this.Subject.Equals(this.UnequalValue).Should().BeFalse(MustBeInequalReason);

    /// <summary>
    /// Checks that <see cref="object.Equals(object)"/> returns <c>false</c> for subject and unequal value.
    /// </summary>
    [Fact]
    public void ObjectEqualsReturnsTrueFalseForUnequalValue()
    {
      ((object)this.Subject).Equals(this.UnequalValue).Should().BeFalse(MustBeInequalReason);
      this.Subject.Should().NotBe(this.UnequalValue, MustBeInequalReason);
    }

    /// <summary>
    /// Checks that equality operator returns <c>false</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void EqualityOperatorReturnsFalseForUnequalValue()
    {
      ApplyExpression(Expression.Equal, this.Subject, this.UnequalValue).Should().BeFalse(MustBeInequalReason);
      ApplyExpression(Expression.Equal, this.UnequalValue, this.Subject).Should().BeFalse(MustBeInequalReason);
    }

    /// <summary>
    /// Checks that inequality operator returns <c>false</c> for subject and equal value.
    /// </summary>
    [Fact]
    public void UnequalityOperatorReturnsTrueForUnequalValue()
    {
      ApplyExpression(Expression.NotEqual, this.Subject, this.UnequalValue).Should().BeTrue(MustBeInequalReason);
      ApplyExpression(Expression.NotEqual, this.UnequalValue, this.Subject).Should().BeTrue(MustBeInequalReason);
    }

    private static bool ApplyExpression<TLhs, TRhs>(
      Func<Expression, Expression, BinaryExpression> expression,
      TLhs lhs,
      TRhs rhs)
      => Expression.Lambda<Func<bool>>(expression(Expression.Constant(lhs, typeof(TLhs)), Expression.Constant(rhs, typeof(TRhs)))).Compile()();
  }
}