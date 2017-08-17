namespace Codelet.DataAnnotations
{
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  /// Various extensions used inside the assembly.
  /// </summary>
  internal static class Extensions
  {
    /// <summary>
    /// Gets the service of type <typeparamref name="TService"/> from the given <paramref name="validationContext"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <param name="validationContext">The validation context.</param>
    /// <returns>The resolved service. Can return null.</returns>
    public static TService GetService<TService>(this ValidationContext validationContext)
      => (TService)validationContext?.GetService(typeof(TService));
  }
}