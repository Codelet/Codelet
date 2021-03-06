﻿namespace Codelet
{
  using System;
  using AutoFixture.Xunit2;
  using Codelet.Testing;
  using FluentAssertions;
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

    public class Or
    {
      [Theory, AutoData]
      public void ReturnsOwnValue(Maybe<int> maybe, Func<int> factory)
        => maybe.Or(factory).Should().Be(maybe.Value, "because it must return own value");

      [Theory, AutoData]
      public void ReturnsArgumentForNone(int value)
        => Maybe<int>.None.Or(() => value).Should().Be(value, "because none has no own value");

      [Theory, AutoData]
      public void ThrowsIfValueFactoryIsNull(Maybe<int> maybe)
        => Assert.Throws<ArgumentNullException>("valueFactory", () => maybe.Or(null));
    }

    public class OrThrow
    {
      [Theory, AutoData]
      public void ReturnsOwnValue(Maybe<int> maybe, Func<InvalidOperationException> factory)
        => maybe.OrThrow(factory).Should().Be(maybe.Value, "because it must return own value");

      [Fact]
      public void ThrowsForNone()
        => Assert.Throws<InvalidOperationException>(() => Maybe<int>.None.OrThrow(() => new InvalidOperationException()));

      [Theory, AutoData]
      public void ThrowsIfExceptionFactoryIsNull(Maybe<int> maybe)
        => Assert.Throws<ArgumentNullException>("exceptionFactory", () => maybe.OrThrow<InvalidOperationException>(null));
    }

    public class OrDefault
    {
      [Theory, AutoData]
      public void ReturnsOriginalValue(Maybe<int> maybe)
        => maybe.OrDefault().Should().Be(maybe.Value, "because it must return own value");

      [Fact]
      public void ReturnsDefaultValueForNone()
        => Maybe<int>.None.OrDefault().Should().Be(default(int), "because none has no own value");
    }
  }
}