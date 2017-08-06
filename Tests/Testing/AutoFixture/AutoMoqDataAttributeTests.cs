namespace Codelet.Testing.AutoFixture
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class AutoMoqDataAttributeTests
  {
    [Theory, AutoMoqData]
    public void GeneratesDataForTest(
      string value,
      Func<int> delegateInstance,
      IEnumerable<double> sequence,
      IDisposable interfaceInstance)
    {
      value.HasContent().Should().BeTrue();

      delegateInstance.Should().NotBeNull();
      delegateInstance().Should().NotBe(0);

      (sequence?.Any() ?? false).Should().BeTrue();

      interfaceInstance.Should().NotBeNull();
      Mock.Get(interfaceInstance).Should().NotBeNull();
    }
  }
}