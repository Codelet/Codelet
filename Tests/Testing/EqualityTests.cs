namespace Codelet.Testing
{
  using System;
  using System.Linq.Expressions;
  using FluentAssertions;
  using Xunit;

  public class EqualityTests
  {
    public class TestEqualityTestsSet
      : EqualityTests<int, int>
    {
      public TestEqualityTestsSet()
        : base(10, 10, 13)
      {
      }
    }
  }
}