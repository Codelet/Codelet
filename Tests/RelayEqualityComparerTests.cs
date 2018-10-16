namespace Codelet
{
  using System;
  using AutoFixture.Xunit2;
  using Codelet.Testing;
  using FluentAssertions;
  using Xunit;

  public class RelayEqualityComparerTests
  {
    public class Constructor
    {
      [Theory, AutoData]
      public void ThrowsIfHasherIsNull(Func<string, string, bool> comparer)
        => Assert.Throws<ArgumentNullException>("hasher", () => new RelayEqualityComparer<string>(null, comparer));

      [Theory, AutoData]
      public void ThrowsIfComparerIsNull(Func<string, int> hasher)
        => Assert.Throws<ArgumentNullException>("comparer", () => new RelayEqualityComparer<string>(hasher, null));
    }

    public new class Equals
    {
      [Theory, AutoData]
      public void ReturnsHashCodeGeneratedByHasher(Func<string, int> hasher, string lhs, string rhs, bool result)
        => new RelayEqualityComparer<string>(hasher, (a, b) => result)
          .Equals(lhs, rhs)
          .Should()
          .Be(result, Because.PassedAsParameter);
    }

    public new class GetHashCode
    {
      [Theory, AutoData]
      public void ReturnsHashCodeGeneratedByHasher(int hashCode, Func<string, string, bool> comparer, string obj)
        => new RelayEqualityComparer<string>(_ => hashCode, comparer)
        .GetHashCode(obj)
        .Should()
        .Be(hashCode, Because.PassedAsParameter);
    }
  }
}