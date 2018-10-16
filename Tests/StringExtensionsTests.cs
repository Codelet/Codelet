namespace Codelet
{
  using System;
  using AutoFixture.Xunit2;
  using Codelet.Testing;
  using FluentAssertions;
  using Xunit;

  public class StringExtensionsTests
  {
    public static object[][] NoContentStrings => Theory.NoContentStrings;

    public class HasContent
    {
      [Theory, AutoData]
      public void ReturnsTrueForContentString(string value)
        => value.HasContent().Should().BeTrue("because input string has value");

      [Theory, MemberData(nameof(NoContentStrings), MemberType = typeof(StringExtensionsTests))]
      public void ReturnsFalseForNoContentStrings(string value)
        => value.HasContent().Should().BeFalse("because input string has no content");
    }

    public class IsNullOrWhiteSpace
    {
      [Theory, MemberData(nameof(NoContentStrings), MemberType = typeof(StringExtensionsTests))]
      public void ReturnsTrueForNoContentStrings(string value)
        => value.IsNullOrWhiteSpace().Should().BeTrue("because input string has no content");

      [Theory, AutoData]
      public void ReturnsFalseForContentString(string value)
        => value.IsNullOrWhiteSpace().Should().BeFalse("because input string has value");
    }

    public class ToMaybe
    {
      [Theory, AutoData]
      public void ReturnsMaybeForContentString(string value)
        => value.ToMaybe().Should().Be(new Maybe<string>(value), "because input string has value");

      [Theory, MemberData(nameof(NoContentStrings), MemberType = typeof(StringExtensionsTests))]
      public void ReturnsNoneForNoContentStrings(string value)
        => value.ToMaybe().Should().Be(Maybe<string>.None, "because input string has no content");
    }

    public class Join
    {
      [Theory, AutoData]
      public void JoinsTheValuesWithSeparator(string[] values, string separator)
        => values.Join(separator).Should().Be(string.Join(separator, values), Because.PassedAsParameter);

      [Theory, AutoData]
      public void ThrowsIfValuesAreNull(string separator)
        => Assert.Throws<ArgumentNullException>("values", () => StringExtensions.Join(null, separator));

      [Theory, AutoData]
      public void DoesntThrowIfSeparatorIsNull(string[] values)
        => values.Join(null);
    }
  }
}