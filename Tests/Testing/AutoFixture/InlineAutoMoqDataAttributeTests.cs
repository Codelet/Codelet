namespace Codelet.Testing.AutoFixture
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class InlineAutoMoqDataAttributeTests
  {
    [Theory, InlineAutoMoqData("111", 13)]
    public void GeneratesDataForTest(
      string inlineValue1,
      int inlineValue2,
      string value,
      Func<int> delegateInstance,
      IEnumerable<double> sequence,
      IDisposable interfaceInstance)
    {
      inlineValue1.Should().Be("111");
      inlineValue2.Should().Be(13);

      value.HasContent().Should().BeTrue();

      delegateInstance.Should().NotBeNull();
      delegateInstance().Should().NotBe(0);

      (sequence?.Any() ?? false).Should().BeTrue();

      interfaceInstance.Should().NotBeNull();
      Mock.Get(interfaceInstance).Should().NotBeNull();
    }
  }
}