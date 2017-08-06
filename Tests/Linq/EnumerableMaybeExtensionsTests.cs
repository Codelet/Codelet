namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class EnumerableMaybeExtensionsTests
  {
    public class ElementAtOrNone
    {
      [Fact]
      public void ReturnsTheSpecifiedElementIfExists()
        => new[] { 1, 2, 3 }.ElementAtOrNone(1).Should().Be((Maybe<int>)2, "because it's the second element");

      [Fact]
      public void ReturnsNoneIfSourceContainsLessElements()
        => new[] { 1, 2 }.ElementAtOrNone(3).Should().Be(Maybe<int>.None, "because source contains less elements");

      [Fact]
      public void ReturnsNoneIfIndexIsNegative()
        => new[] { 1, 2 }.ElementAtOrNone(-1).Should().Be(Maybe<int>.None, "because index is negative");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).ElementAtOrNone(10));
    }

    public class FirstOrNoneWithoutPredicate
    {
      [Fact]
      public void ReturnsFirstElementIfExists()
        => new[] { 1, 2 }.FirstOrNone().Should().Be((Maybe<int>)1, "because it's the first element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoElements()
        => new int[0].FirstOrNone().Should().Be(Maybe<int>.None, "because source contains no elements");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).FirstOrNone());
    }

    public class FirstOrNoneWithPredicate
    {
      [Fact]
      public void ReturnsFirstCorrespondingElementIfExists()
        => new[] { 1, 2, 4, 3 }.FirstOrNone(x => x % 2 == 0).Should().Be((Maybe<int>)2, "because it's the first even element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoCorrespondingElements()
        => new[] { 1, 2 }.FirstOrNone(_ => false).Should().Be(Maybe<int>.None, "because source contains no corresponding elements");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).FirstOrNone(Mock.Of<Func<int, bool>>()));

      [Fact]
      public void ThrowsIfPredicateIsNull()
        => Assert.Throws<ArgumentNullException>("predicate", () => new int[0].FirstOrNone(null));
    }

    public class LastOrNoneWithoutPredicate
    {
      [Fact]
      public void ReturnsLastElementIfExists()
        => new[] { 1, 2 }.LastOrNone().Should().Be((Maybe<int>)2, "because it's the last element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoElements()
        => new int[0].LastOrNone().Should().Be(Maybe<int>.None, "because source contains no elements");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).LastOrNone());
    }

    public class LastOrNoneWithPredicate
    {
      [Fact]
      public void ReturnsLastCorrespondingElementIfExists()
        => new[] { 1, 2, 4, 3 }.LastOrNone(x => x % 2 == 0).Should().Be((Maybe<int>)4, "because it's the last even element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoCorrespondingElements()
        => new[] { 1, 2 }.LastOrNone(_ => false).Should().Be(Maybe<int>.None, "because source contains no corresponding elements");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).LastOrNone(Mock.Of<Func<int, bool>>()));

      [Fact]
      public void ThrowsIfPredicateIsNull()
        => Assert.Throws<ArgumentNullException>("predicate", () => new int[0].LastOrNone(null));
    }

    public class SingletOrNoneWithoutPredicate
    {
      [Fact]
      public void ReturnsSingleElementIfExists()
        => new[] { 1 }.SingleOrNone().Should().Be((Maybe<int>)1, "because it's the only element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoElements()
        => new int[0].SingleOrNone().Should().Be(Maybe<int>.None, "because source contains no elements");

      [Fact]
      public void ThrowsIfSourceContainsMoreThanOneElement()
        => Assert.Throws<InvalidOperationException>(() => new[] { 1, 2 }.SingleOrNone());

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).SingleOrNone());
    }

    public class SingleOrNoneWithPredicate
    {
      [Fact]
      public void ReturnsSingleCorrespondingElementIfExists()
        => new[] { 1, 3, 6, 3, 5 }.SingleOrNone(x => x % 2 == 0).Should().Be((Maybe<int>)6, "because it's the only even element");

      [Fact]
      public void ReturnsNoneIfSourceContainsNoCorrespondingElements()
        => new[] { 1, 3 }.SingleOrNone(x => x % 2 == 0).Should().Be(Maybe<int>.None, "because source contains no even elements");

      [Fact]
      public void ThrowsIfSourceContainsMoreThanOneCorrespondingElement()
        => Assert.Throws<InvalidOperationException>(() => new[] { 2, 4 }.SingleOrNone(x => x % 2 == 0));

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<int>)null).SingleOrNone(Mock.Of<Func<int, bool>>()));

      [Fact]
      public void ThrowsIfPredicateIsNull()
        => Assert.Throws<ArgumentNullException>("predicate", () => new int[0].SingleOrNone(null));
    }
  }
}