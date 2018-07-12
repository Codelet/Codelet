namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Codelet.Testing;
  using Codelet.Testing.AutoFixture;
  using FluentAssertions;
  using Moq;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class DictionaryExtensionsTests
  {
    public class GetOrCreate
    {
      [Theory, AutoData]
      public void GetsTheAssosiatedValue(
        Dictionary<string, int> dictionary,
        string key,
        int value,
        Mock<Func<int>> factory)
      {
        dictionary[key] = value;

        dictionary.GetOrCreate(key, factory.Object).Should().Be(value, "because value is in the dictionary");
        factory.Verify(m => m(), Times.Never);
      }

      [Theory, AutoData]
      public void CreatesAndAssosiatesValue(Dictionary<string, int> dictionary, string key, int value)
        => dictionary.GetOrCreate(key, () => value).Should().Be(value, "because value is in the dictionary");

      [Theory, AutoData]
      public void ThrowsIfDictionaryIsNull(string key, Func<int> factory)
        => Assert.Throws<ArgumentNullException>("dictionary", () => DictionaryExtensions.GetOrCreate(null, key, factory));

      [Theory, AutoData]
      public void ThrowsIfFactoryIsNull(IDictionary<string, int> dictionary, string key)
        => Assert.Throws<ArgumentNullException>("factory", () => dictionary.GetOrCreate(key, null));
    }

    public class GetOrCreateAsync
    {
      [Theory, AutoData]
      public async Task GetsTheAssosiatedValue(
        Dictionary<string, int> dictionary,
        string key,
        int value,
        Mock<Func<Task<int>>> factory)
      {
        dictionary[key] = value;

        (await dictionary.GetOrCreateAsync(key, factory.Object)).Should().Be(value, "because value is in the dictionary");
        factory.Verify(m => m(), Times.Never);
      }

      [Theory, AutoData]
      public async Task CreatesAndAssosiatesValue(Dictionary<string, int> dictionary, string key, int value)
      {
        (await dictionary.GetOrCreateAsync(key, () => Task.FromResult(value))).Should().Be(value, "because value is in the dictionary");
      }

      [Theory, AutoData]
      public Task ThrowsIfDictionaryIsNull(string key, Func<Task<int>> factory)
        => Assert.ThrowsAsync<ArgumentNullException>("dictionary", () => DictionaryExtensions.GetOrCreateAsync(null, key, factory));

      [Theory, AutoData]
      public Task ThrowsIfFactoryIsNull(IDictionary<string, int> dictionary, string key)
        => Assert.ThrowsAsync<ArgumentNullException>("factory", () => dictionary.GetOrCreateAsync(key, null));
    }

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