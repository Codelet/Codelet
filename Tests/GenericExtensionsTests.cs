namespace Codelet
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using AutoFixture.Xunit2;
  using Codelet.Linq;
  using FluentAssertions;
  using Xunit;

  public class GenericExtensionsTests
  {
    public class FlattenWithSingleDescendantSelector
    {
      [Theory, AutoData]
      public void ReturnsEmptyCollectionForNull(Func<string, string> selector)
        => ((string)null)
        .Flatten(selector)
        .Should().BeEquivalentTo(Enumerable.Empty<string>(), "because initial target is null");

      [Fact]
      public void ReturnsFlattenedSequence()
        => ((int?)0)
        .Flatten(value => value < 9 ? value + 1 : null)
        .Should().BeEquivalentTo(Enumerable.Range(0, 10), "because flatten should generate the sequence from zero to nine");
    }

    public class FlattenWithMultipleDescendantsSelector
    {
      [Theory, AutoData]
      public void ReturnsEmptyCollectionForNull(Func<string, IEnumerable<string>> selector)
        => ((string)null)
        .Flatten(selector)
        .Should().BeEquivalentTo(Enumerable.Empty<string>(), "because initial target is null");

      [Fact]
      public void ReturnsFlattenedSequence()
        => ((int?)0)
        .Flatten(value => value == 0 ? Enumerable.Range(1, 3).Cast<int?>().Concat((int?)null) : Enumerable.Empty<int?>())
        .Should().BeEquivalentTo(Enumerable.Range(0, 4), "because flatten should generate the sequence from zero to three");
    }
  }
}