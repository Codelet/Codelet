namespace Codelet.Functional
{
  using System;
  using System.Threading.Tasks;
  using AutoFixture.Xunit2;
  using Codelet.Testing;
  using FluentAssertions;
  using Moq;
  using Xunit;

  public class TaskLinqExtensionsTests
  {
    public class Select
    {
      [Theory, AutoData]
      public async Task ReturnsNewValue(Task<int> task, string innerResult)
      {
        var result = task.Select(value =>
        {
          value.Should().Be(task.Result, Because.PassedAsParameter);
          return innerResult;
        });

        (await result).Should().Be(innerResult, "because the task result was projected to this value");
      }

      [Theory, AutoData]
      public async Task ThrowsIfTaskIsNull(Func<int, string> selector)
        => await Assert.ThrowsAsync<ArgumentNullException>("task", () => ((Task<int>)null).Select(selector));

      [Theory, AutoData]
      public async Task ThrowsIfSelectorIsNull(Task<int> task)
        => await Assert.ThrowsAsync<ArgumentNullException>("selector", () => task.Select((Func<int, string>)null));
    }

    public class SelectTask
    {
      [Theory, AutoData]
      public async Task ReturnsNewValue(Task<int> task, Task<string> innerResult)
      {
        var result = task.Select(value =>
        {
          value.Should().Be(task.Result, Because.PassedAsParameter);
          return innerResult;
        });

        (await result).Should().Be(innerResult.Result, "because the task result was projected to this value");
      }

      [Theory, AutoData]
      public async Task ThrowsIfTaskIsNull(Func<int, Task<string>> selector)
        => await Assert.ThrowsAsync<ArgumentNullException>("task", () => ((Task<int>)null).Select(selector));

      [Theory, AutoData]
      public async Task ThrowsIfSelectorIsNull(Task<int> task)
        => await Assert.ThrowsAsync<ArgumentNullException>("selector", () => task.Select((Func<int, Task<string>>)null));
    }

    public class SelectManyTask
    {
      [Theory, AutoData]
      public async Task ReturnsNewValue(Task<int> task, Task<double> innerResult, string outerResult)
      {
        var result = task.SelectMany(
          value =>
          {
            value.Should().Be(task.Result, Because.PassedAsParameter);
            return innerResult;
          },
          (value, innerValue) =>
          {
            value.Should().Be(task.Result, Because.PassedAsParameter);
            innerValue.Should().Be(innerResult.Result, Because.PassedAsParameter);
            return outerResult;
          });

        (await result).Should().Be(outerResult, "because the task result was projected to this value");
      }

      [Theory, AutoData]
      public async Task ThrowsIfTaskIsNull(Func<int, Task<double>> innerSelector, Func<int, double, string> outerSelector)
        => await Assert.ThrowsAsync<ArgumentNullException>("task", () => ((Task<int>)null).SelectMany(innerSelector, outerSelector));

      [Theory, AutoData]
      public async Task ThrowsIfInnerSelectorIsNull(Task<int> task)
        => await Assert.ThrowsAsync<ArgumentNullException>(
          "innerSelector",
          () => task.SelectMany((Func<int, Task<double>>)null, Mock.Of<Func<int, double, string>>()));

      [Theory, AutoData]
      public async Task ThrowsIfOuterSelectorIsNull(Task<int> task)
        => await Assert.ThrowsAsync<ArgumentNullException>(
          "outerSelector",
          () => task.SelectMany(Mock.Of<Func<int, Task<double>>>(), (Func<int, double, string>)null));
    }

    public class SelectManyMaybe
    {
      [Theory, AutoData]
      public async Task ReturnsNewValue(Task<int> task, Maybe<double> innerResult, string outerResult)
      {
        var result = task.SelectMany(
          value =>
          {
            value.Should().Be(task.Result, Because.PassedAsParameter);
            return innerResult;
          },
          (value, innerValue) =>
          {
            value.Should().Be(task.Result, Because.PassedAsParameter);
            innerValue.Should().Be(innerResult.Value, Because.PassedAsParameter);
            return outerResult;
          });

        (await result).Value.Should().Be(outerResult, "because the task result was projected to this value");
      }

      [Theory, AutoData]
      public async Task ThrowsIfTaskIsNull(Func<int, Maybe<double>> innerSelector, Func<int, double, string> outerSelector)
        => await Assert.ThrowsAsync<ArgumentNullException>("task", () => ((Task<int>)null).SelectMany(innerSelector, outerSelector));

      [Theory, AutoData]
      public async Task ThrowsIfInnerSelectorIsNull(Task<int> task, Func<int, double, string> outerSelector)
        => await Assert.ThrowsAsync<ArgumentNullException>("innerSelector", () => task.SelectMany((Func<int, Maybe<double>>)null, outerSelector));

      [Theory, AutoData]
      public async Task ThrowsIfOuterSelectorIsNull(Task<int> task, Func<int, Maybe<double>> innerSelector)
        => await Assert.ThrowsAsync<ArgumentNullException>("outerSelector", () => task.SelectMany(innerSelector, (Func<int, double, string>)null));
    }
  }
}