namespace Codelet.Testing.AutoFixture
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using Castle.Core.Internal;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class MemberAutoMoqDataAttributeTests
  {
    public static object[][] Data
      => new[]
      {
        new object[] { "111", 13 },
        new object[] { "222", 15 },
      };

    [Theory, MemberAutoMoqData(nameof(Data), MemberType = typeof(MemberAutoMoqDataAttributeTests))]
    public void GeneratesDataForTest(
      string inlineValue1,
      int inlineValue2,
      string value,
      Func<int> delegateInstance,
      IEnumerable<double> sequence,
      IDisposable interfaceInstance)
    {
      inlineValue1.Should().BeOneOf("111", "222");
      inlineValue2.Should().BeOneOf(13, 15);

      value.HasContent().Should().BeTrue();

      delegateInstance.Should().NotBeNull();
      delegateInstance().Should().NotBe(0);

      (sequence?.Any() ?? false).Should().BeTrue();

      interfaceInstance.Should().NotBeNull();
      Mock.Get(interfaceInstance).Should().NotBeNull();

      var attribute = new StackTrace()
        .GetFrame(0)
        .GetMethod()
        .GetAttribute<MemberAutoMoqDataAttribute>();

      attribute.Should().NotBeNull();

      attribute.MemberType.Should().Be(typeof(MemberAutoMoqDataAttributeTests));
    }
  }
}