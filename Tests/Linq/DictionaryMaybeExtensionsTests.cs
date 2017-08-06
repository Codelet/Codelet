namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using FluentAssertions;
  using Xunit;

  public class DictionaryMaybeExtensionsTests
  {
    public class TryGetValue
    {
      [Fact]
      public void ReturnsValueForExistingKey()
        => new Dictionary<string, int> { { "key", 10 } }
          .TryGetValue("key")
          .Should()
          .Be((Maybe<int>)10, "because the key is not in dictionary");

      [Fact]
      public void ReturnsNoneForInexistingKey()
        => new Dictionary<string, int>()
        .TryGetValue("key")
        .Should()
        .Be(Maybe<int>.None, "because the key is not in dictionary");

      [Fact]
      public void ThrowsIfDictionartyIsNull()
        => Assert.Throws<ArgumentNullException>("dictionary", () => ((Dictionary<string, int>)null).TryGetValue("key"));
    }
  }
}