#pragma warning disable SA1649 // File name should match first type name

namespace Codelet.Razor
{
  using System;
  using System.Threading.Tasks;

  /// <summary>
  /// Base class for compiled Razor template.
  /// </summary>
  /// <typeparam name="TModel">The type of the model.</typeparam>
  public abstract class RazorTemplate<TModel>
    : RazorTemplate
  {
    /// <inheritdoc cref="RazorTemplate.Model" />
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    protected new TModel Model { get; }

    /// <inheritdoc />
    public override Task ExecuteAsync()
    {
      throw new InvalidOperationException("Should not be used directly.");
    }
  }
}

#pragma warning restore SA1649 // File name should match first type name