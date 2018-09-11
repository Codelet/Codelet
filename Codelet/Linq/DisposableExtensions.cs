namespace Codelet.Linq
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Extension methods for <see cref="IDisposable"/>.
  /// </summary>
  public static class DisposableExtensions
  {
    /// <summary>
    /// Merges the specified list of disposables into a single disposable..
    /// </summary>
    /// <param name="disposables">The disposables.</param>
    /// <returns>
    /// The single disposable that disposes all instances in <paramref name="disposables"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name ="disposables" /> == <c>null</c>.</exception>
    public static IDisposable Merge(this IEnumerable<IDisposable> disposables)
      => new MergedDisposables(disposables ?? throw new ArgumentNullException(nameof(disposables)));

    private class MergedDisposables : IDisposable
    {
      public MergedDisposables(IEnumerable<IDisposable> disposables)
        => this.Disposables = disposables;

      private IEnumerable<IDisposable> Disposables { get; }

      public void Dispose()
      {
        foreach (var disposable in this.Disposables)
        {
          disposable?.Dispose();
        }
      }
    }
  }
}