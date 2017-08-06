namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using Ploeh.AutoFixture.Xunit2;
  using Xunit;

  public class EnumerableExtensionsTests
  {
    public class AddRangeSequence
    {
      [Theory, AutoData]
      public void AddsItemsToCollection(ICollection<int> collection, IEnumerable<int> items)
      {
        var originalCollection = collection.ToArray();
        collection.AddRange(items);
        collection.ShouldBeEquivalentTo(originalCollection.Concat(items), "because items were added to original collection");
      }

      [Theory, AutoData]
      public void ThrowsIfCollectionIsNull(IEnumerable<int> items)
        => Assert.Throws<ArgumentNullException>("collection", () => EnumerableExtension.AddRange<int>(null, items));

      [Theory, AutoData]
      public void ThrowsIfItemsIsNull(ICollection<int> collection)
        => Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((IEnumerable<int>)null));
    }

    public class AddRangeArray
    {
      [Theory, AutoData]
      public void AddsItemsToCollection(ICollection<int> collection, int[] items)
      {
        var originalCollection = collection.ToArray();
        collection.AddRange(items);
        collection.ShouldBeEquivalentTo(originalCollection.Concat(items), "because items were added to original collection");
      }

      [Theory, AutoData]
      public void ThrowsIfCollectionIsNull(int[] items)
        => Assert.Throws<ArgumentNullException>("collection", () => EnumerableExtension.AddRange(null, items));

      [Theory, AutoData]
      public void ThrowsIfItemsIsNull(ICollection<int> collection)
        => Assert.Throws<ArgumentNullException>("items", () => collection.AddRange(null));
    }

    public class RemoveSequence
    {
      [Theory, AutoData]
      public void RemovesItemsFromCollection(ICollection<int> collection, IEnumerable<int> items)
      {
        var originalCollection = collection.ToArray();
        collection.RemoveAll(originalCollection.Take(2));
        collection.ShouldBeEquivalentTo(originalCollection.Skip(2), "because items were removed from original collection");
      }

      [Theory, AutoData]
      public void ThrowsIfCollectionIsNull(IEnumerable<int> items)
        => Assert.Throws<ArgumentNullException>("collection", () => EnumerableExtension.AddRange<int>(null, items));

      [Theory, AutoData]
      public void ThrowsIfItemsIsNull(ICollection<int> collection)
        => Assert.Throws<ArgumentNullException>("items", () => collection.AddRange((IEnumerable<int>)null));
    }

    public class RemoveByPredicate
    {
      [Theory, AutoData]
      public void RemovesItemsFromCollection(ICollection<int> collection)
      {
        var originalCollection = collection.ToArray();
        collection.RemoveAll(originalCollection.Take(2).Contains);
        collection.ShouldBeEquivalentTo(originalCollection.Skip(2), "because items were removed from original collection");
      }

      [Theory, AutoData]
      public void ThrowsIfCollectionIsNull(Func<int, bool> predicate)
        => Assert.Throws<ArgumentNullException>("collection", () => EnumerableExtension.RemoveAll<int>(null, predicate));

      [Theory, AutoData]
      public void ThrowsIfPredicateIsNull(ICollection<int> collection)
        => Assert.Throws<ArgumentNullException>("predicate", () => collection.RemoveAll((Func<int, bool>)null));
    }

    public class RemoveArray
    {
      [Theory, AutoData]
      public void RemovesItemsFromCollection(ICollection<int> collection, int[] items)
      {
        var originalCollection = collection.ToArray();
        collection.RemoveAll(collection.Take(2).ToArray());
        collection.ShouldBeEquivalentTo(originalCollection.Skip(2), "because items were removed from original collection");
      }

      [Theory, AutoData]
      public void ThrowsIfCollectionIsNull(int[] items)
        => Assert.Throws<ArgumentNullException>("collection", () => EnumerableExtension.AddRange(null, items));

      [Theory, AutoData]
      public void ThrowsIfItemsIsNull(ICollection<int> collection)
        => Assert.Throws<ArgumentNullException>("items", () => collection.AddRange(null));
    }

    public class Concat
    {
      [Theory, AutoData]
      public void ProducesNewSequenceWithAttachedElement(IEnumerable<int> source, int value)
      {
        var result = source.Concat(value);
        var expected = source.ToList();
        expected.Add(value);

        result.AsEnumerable().ShouldBeEquivalentTo(expected, "because item was concatenated to the sequence");
      }

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.Concat(null, string.Empty));
    }

    public class Distinct
    {
      [Fact]
      public void DistinctsElementsThatHaveEqualProjections()
        => Enumerable
        .Range(1, 10)
        .Distinct(item => item < 5 ? item : 5)
        .ShouldBeEquivalentTo(Enumerable.Range(1, 5), "because 5 and larger elements are considered equal");

      [Theory, AutoData]
      public void ThrowsIfSourceIsNull(Func<int, string> selector)
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.Distinct(null, selector));

      [Theory, AutoData]
      public void ThrowsIfSelectorIsNull(IEnumerable<int> source)
        => Assert.Throws<ArgumentNullException>("selector", () => source.Distinct((Func<int, string>)null));
    }

    public class None
    {
      [Fact]
      public void ReturnsTrueForEmptyCollection()
        => Enumerable.Empty<string>().None().Should().BeTrue("because collection is empty");

      [Fact]
      public void ReturnsFalseForCollectionWithElements()
        => Enumerable.Range(0, 3).None().Should().BeFalse("because collection is not empty");

      [Fact]
      public void ThrowsIfSourceIsNull()
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.None<string>(null));
    }

    public class NoneWithPredicate
    {
      [Fact]
      public void ReturnsTrueForCollectionWithoutCorrespondingElements()
        => Enumerable.Range(0, 3)
        .None(item => item >= 3)
        .Should()
        .BeTrue("because collection has no elements larger then 2");

      [Fact]
      public void ReturnsFalseForCollectionWithCorrespondingElements()
        => Enumerable.Range(0, 4)
        .None(item => item >= 3)
        .Should()
        .BeFalse("because collection has elements larger then 2");

      [Theory, AutoData]
      public void ThrowsIfSourceIsNull(Func<string, bool> predicate)
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.None(null, predicate));

      [Theory, AutoData]
      public void ThrowsIfPredicateIsNull(IEnumerable<string> source)
        => Assert.Throws<ArgumentNullException>("predicate", () => source.None(null));
    }

    public class SelectWithParameter
    {
      [Fact]
      public void ProjectsElementsWithArgument()
        => Enumerable
        .Range(0, 3)
        .Select((item, extension) => item + extension, 10)
        .ShouldBeEquivalentTo(Enumerable.Range(10, 3), "because each element in the sequence was increased by 10");

      [Theory, AutoData]
      public void ThrowsIfSourceIsNull(Func<int, string, bool> selector, string argument)
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.Select(null, selector, argument));

      [Theory, AutoData]
      public void ThrowsIfSelectorIsNull(IEnumerable<int> source, string argument)
        => Assert.Throws<ArgumentNullException>("selector", () => source.Select((Func<int, string, bool>)null, argument));
    }

    public class ForEach
    {
      [Theory, AutoData]
      public void InvokesActionForEachElelemntInCollection(IEnumerable<int> source)
      {
        var result = new List<int>();
        source.ForEach(result.Add);

        result.AsEnumerable().ShouldBeEquivalentTo(source, "because each item had to be moved to result collection");
      }

      [Theory, AutoData]
      public void ThrowsIfSourceIsNull(Action<string> action)
        => Assert.Throws<ArgumentNullException>("source", () => EnumerableExtension.ForEach(null, action));

      [Theory, AutoData]
      public void ThrowsIfActionIsNull(IEnumerable<string> source)
        => Assert.Throws<ArgumentNullException>("action", () => source.ForEach(null));
    }
  }
}