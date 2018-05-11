namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Codelet.Testing;
  using Codelet.Testing.AutoFixture;
  using FluentAssertions;
  using Moq;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class DictionaryExtensionsTests
  {
    public class AsReadOnly
    {
      [Theory, AutoData]
      public void ConvertsToReadOnlyDictionary(IDictionary<string, int> dictionary, string key, int value)
      {
        var readOnlyDictionary = dictionary.AsReadOnly();
        readOnlyDictionary.ShouldBeEquivalentTo(dictionary, Because.PassedAsParameter);

        dictionary[key] = value;
        readOnlyDictionary.ShouldBeEquivalentTo(dictionary, "because read only dictionary should reflect changes");
      }

      [Fact]
      public void ThrowsIfDictionaryIsNull()
        => Assert.Throws<ArgumentNullException>("dictionary", () => DictionaryExtensions.AsReadOnly<string, int>(null));
    }

    public class KeyValuePairsToDictionary
    {
      [Theory, AutoMoqData]
      public void ConvertsToDictionary(IEnumerable<KeyValuePair<string, int>> source)
        => source.ToDictionary().AsEnumerable().ShouldBeEquivalentTo(source, Because.PassedAsParameter);

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<KeyValuePair<string, int>>)null).ToDictionary());
    }

    public class KeyValuePairsToDictionaryWithComparer
    {
      [Theory, AutoMoqData]
      public void ConvertsToDictionary(KeyValuePair<string, int> item, Mock<IEqualityComparer<string>> comparer)
      {
        comparer.Setup(m => m.GetHashCode(It.IsAny<string>())).Returns<string>(value => 0);
        comparer.Setup(m => m.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((lhs, rhs) => false);

        var source = Enumerable.Repeat(item, 3);
        var dictionary = source.ToDictionary(comparer.Object);

        dictionary.AsEnumerable().ShouldBeEquivalentTo(source, Because.PassedAsParameter);
      }

      [Theory, AutoMoqData]
      public void ThrowsIfSourceIsNull(IEqualityComparer<string> comparer)
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<KeyValuePair<string, int>>)null).ToDictionary(comparer));

      [Theory, AutoData]
      public void ThrowsIfComparerIsNull(IEnumerable<KeyValuePair<string, int>> source)
        => Assert.Throws<ArgumentNullException>("comparer", () => source.ToDictionary(null));
    }

    /*
    public class ValueTuplesToDictionary
    {
      [Theory, AutoMoqData]
      public void ConvertsToDictionary(IEnumerable<(string, int)> source)
        => source.ToDictionary().Select(entry => (entry.Key, entry.Value)).ShouldBeEquivalentTo(source, Because.PassedAsParameter);

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<ValueTuple<string, int>>)null).ToDictionary());
    }

    public class ValueTuplesToDictionaryWithComparer
    {
      [Theory, AutoMoqData]
      public void ConvertsToDictionary((string, int) item, Mock<IEqualityComparer<string>> comparer)
      {
        comparer.Setup(m => m.GetHashCode(It.IsAny<string>())).Returns<string>(value => 0);
        comparer.Setup(m => m.Equals(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((lhs, rhs) => false);

        var source = Enumerable.Repeat(item, 3);
        var dictionary = source.ToDictionary(comparer.Object);

        dictionary.Select(entry => (entry.Key, entry.Value)).ShouldBeEquivalentTo(source, Because.PassedAsParameter);
      }

      [Theory, AutoMoqData]
      public void ThrowsIfSourceIsNull(IEqualityComparer<string> comparer)
        => Assert.Throws<ArgumentNullException>("source", () => ((IEnumerable<ValueTuple<string, int>>)null).ToDictionary(comparer));

      [Theory, AutoData]
      public void ThrowsIfComparerIsNull(IEnumerable<(string, int)> source)
        => Assert.Throws<ArgumentNullException>("comparer", () => source.ToDictionary(null));
    }
    */
  }
}