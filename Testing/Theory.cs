namespace Codelet.Testing
{
  using System.Linq;

  /// <summary>
  /// Helper methods to create data for XUnit theories.
  /// </summary>
  public static class Theory
  {
    /// <summary>
    /// Gets the no content strings.
    /// </summary>
    public static object[][] NoContentStrings => Data(null, string.Empty, "   ");

    /// <summary>
    /// Generates the array of single value data sets.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The data values (each item - separate test data set).</param>
    /// <returns>The data sets.</returns>
    public static object[][] Data<T>(params T[] data)
      => data.Select(item => new object[] { item }).ToArray();
  }
}