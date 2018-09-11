namespace Codelet.Linq
{
  using System;
  using Codelet.Testing.AutoFixture;
  using Moq;
  using Xunit;

  public class DisposableExtensionsTests
  {
    public class Merge
    {
      [Theory, AutoMoqData]
      public void DisposesAllInstances(IDisposable[] disposables)
      {
        disposables.Merge().Dispose();

        foreach (var disposable in disposables)
        {
          Mock.Get(disposable).Verify(m => m.Dispose(), Times.Once);
        }
      }

      [Fact]
      public void ThrowsIfDisposablesIsNull()
        => Assert.Throws<ArgumentNullException>("disposables", () => DisposableExtensions.Merge(null));
    }
  }
}