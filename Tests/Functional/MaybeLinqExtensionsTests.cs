namespace Codelet.Functional
{
  using System;
  using AutoFixture.Xunit2;
  using Codelet.Testing;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class MaybeLinqExtensionsTests
  {
    public class Select
    {
      [Theory, AutoData]
      public void ReturnsNewValue(Maybe<int> maybe, string innerResult)
      {
        var result = maybe.Select(value =>
        {
          value.Should().Be(maybe.Value, Because.PassedAsParameter);
          return innerResult;
        });

        result.Value.Should().Be(innerResult, "because the Maybe result was projected to this value");
      }

      [Theory, AutoData]
      public void ReturnsNoneIfSelectorReturnsNull(Maybe<int> maybe)
      {
        var result = maybe.Select(value =>
        {
          value.Should().Be(maybe.Value, Because.PassedAsParameter);
          return (string)null;
        });

        result.Should().Be(Maybe<string>.None, "because the Maybe result was projected to null");
      }

      [Theory, AutoData]
      public void ThrowsIfSelectorIsNull(Maybe<int> maybe)
        => Assert.Throws<ArgumentNullException>("selector", () => maybe.Select((Func<int, string>)null));
    }

    public class SelectMaybe
    {
      [Theory, AutoData]
      public void ReturnsNewValue(Maybe<int> maybe, Maybe<string> innerResult)
      {
        var result = maybe.Select(value =>
        {
          value.Should().Be(maybe.Value, Because.PassedAsParameter);
          return innerResult;
        });

        result.Value.Should().Be(innerResult.Value, "because the Maybe result was projected to this value");
      }

      [Theory, AutoData]
      public void ThrowsIfSelectorIsNull(Maybe<int> maybe)
        => Assert.Throws<ArgumentNullException>("selector", () => maybe.Select((Func<int, Maybe<string>>)null));
    }

    public class SelectManyMaybe
    {
      [Theory, AutoData]
      public void ReturnsNewValue(Maybe<int> maybe, Maybe<double> innerResult, string outerResult)
      {
        var result = maybe.SelectMany(
          value =>
          {
            value.Should().Be(maybe.Value, Because.PassedAsParameter);
            return innerResult;
          },
          (value, innerValue) =>
          {
            value.Should().Be(maybe.Value, Because.PassedAsParameter);
            innerValue.Should().Be(innerResult.Value, Because.PassedAsParameter);
            return outerResult;
          });

        result.Value.Should().Be(outerResult, "because the Maybe result was projected to this value");
      }

      [Theory, AutoData]
      public void ThrowsIfInnerSelectorIsNull(Maybe<int> maybe, Func<int, double, string> outerSelector)
        => Assert.Throws<ArgumentNullException>("innerSelector", () => maybe.SelectMany(null, outerSelector));

      [Theory, AutoData]
      public void ThrowsIfOuterSelectorIsNull(Maybe<int> maybe, Func<int, Maybe<double>> innerSelector)
        => Assert.Throws<ArgumentNullException>("outerSelector", () => maybe.SelectMany<int, double, string>(innerSelector, null));
    }

    public class Where
    {
      [Theory, AutoData]
      public void ReturnsOriginalMaybeIfSelectorReturnsTrue(Maybe<int> maybe)
      {
        var result = maybe.Where(value =>
        {
          value.Should().Be(maybe.Value, Because.PassedAsParameter);
          return true;
        });

        result.Should().Be(maybe, "because the Maybe passed the filter");
      }

      [Theory, AutoData]
      public void ReturnsNoneIfSelectorReturnsFalse(Maybe<int> maybe)
      {
        var result = maybe.Where(value =>
        {
          value.Should().Be(maybe.Value, Because.PassedAsParameter);
          return false;
        });

        result.Should().Be(Maybe<int>.None, "because the Maybe didn't pass the filter");
      }

      [Fact]
      public void ReturnsNoneIfOriginalMaybeIsNone()
      {
        var predicate = new Mock<Func<int, bool>>();

        var result = Maybe<int>.None.Where(predicate.Object);

        predicate.Verify(m => m(It.IsAny<int>()), Times.Never());
        result.Should().Be(Maybe<int>.None, "because the Maybe is None");
      }

      [Theory, AutoData]
      public void ThrowsIfPredicateIsNull(Maybe<int> maybe)
        => Assert.Throws<ArgumentNullException>("predicate", () => maybe.Where(null));
    }
  }
}