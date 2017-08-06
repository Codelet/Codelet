namespace Codelet
{
  using System;
  using Codelet.Testing;
  using FluentAssertions;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class MaybeTests
  {
    public class None
    {
      [Fact]
      public void HasValueReturnsFalse()
        => Maybe<int>.None.HasValue.Should().BeFalse("because maybe has no value");

      [Fact]
      public void ThrowsOnValueAccess()
        => Assert.Throws<InvalidOperationException>(() => Maybe<int>.None.Value);

    }

    public class Constructor
    {
      [Theory, AutoData]
      public void Constructs(int value)
      {
        var maybe = new Maybe<int>(value);
        maybe.HasValue.Should().BeTrue("because maybe has value");
        maybe.Value.Should().Be(value, Because.PassedAsParameter);
      }

      [Fact]
      public void ThrowsIfValueIsNull()
        => Assert.Throws<ArgumentNullException>("value", () => new Maybe<object>(null));
    }

    public class Equality
      : EqualityTests<Maybe<int>, Maybe<int>>
    {
      public Equality()
        : base(10, 10, 5)
      {
      }
    }

    public class Conversion
    {
      [Theory, AutoData]
      public void ReturnsNotNoneForValue(int value)
        => ((Maybe<int>)value).Should().Be(new Maybe<int>(value), "because it's converted from meaningful value");

      [Fact]
      public void ReturnsNoneForNull()
        => ((Maybe<object>)null)
        .Should().Be(Maybe<object>.None, "because it's converted from null");
    }
  }
}