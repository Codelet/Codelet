namespace Codelet
{
  using System.Collections.Generic;
  using FluentAssertions;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class MaybeExtensionsTests
  {
    public static IEnumerable<object[]> Booleans { get; } = new[] { new object[] { false }, new object[] { true } };

    public class OrTrue
    {
      [Theory, MemberData(nameof(Booleans), MemberType = typeof(MaybeExtensionsTests))]
      public void ReturnsOriginalValue([Frozen] bool value)
        => new Maybe<bool>(value).OrTrue().Should().Be(value, "because it must return own value");

      [Fact]
      public void ReturnsTrueForNone()
        => Maybe<bool>.None.OrTrue().Should().Be(true, "because none has no own value");
    }

    public class OrFalse
    {
      [Theory, MemberData(nameof(Booleans), MemberType = typeof(MaybeExtensionsTests))]
      public void ReturnsOriginalValue([Frozen] bool value)
        => new Maybe<bool>(value).OrFalse().Should().Be(value, "because it must return own value");

      [Fact]
      public void ReturnsFalseForNone()
        => Maybe<bool>.None.OrFalse().Should().Be(false, "because none has no own value");
    }

    public class OrEmpty
    {
      [Theory, AutoData]
      public void ReturnsOriginalValue(Maybe<string> maybe)
        => maybe.OrEmpty().Should().Be(maybe.Value, "because it must return own value");

      [Fact]
      public void ReturnsEmptyStringForNone()
        => Maybe<string>.None.OrEmpty().Should().Be(string.Empty, "because none has no own value");
    }

    public class Unwrap
    {
      [Theory, AutoData]
      public void ReturnsOriginalValue(Maybe<Maybe<int>> maybe)
        => maybe.Unwrap().Should().Be(maybe.Value, "because it must return own value");

      [Fact]
      public void ReturnsNoneForNone()
        => Maybe<Maybe<int>>.None.Unwrap().Should().Be(Maybe<int>.None, "because none has no own value");

      [Fact]
      public void ReturnsNoneForInnerNone()
        => new Maybe<Maybe<int>>(Maybe<int>.None).Unwrap().Should().Be(Maybe<int>.None, "because none has no own value");
    }
  }
}