namespace Codelet.Testing
{
  /// <summary>
  /// Typical "because" reasons for fluid asserts.
  /// </summary>
  public static class Because
  {
    /// <summary>
    /// The value was passed as parameter but doesn't match during check.
    /// </summary>
    public const string PassedAsParameter = "because it was passed as parameter.";

    /// <summary>
    /// The parameter value was null and it's not acceptable.
    /// </summary>
    public const string ParameterNullValueIsNotAccepted = "because parameter null value is not accepted";

    /// <summary>
    /// The parameter value was empty string or white space and it's not acceptable.
    ///  </summary>
    public const string ParameterEmptyStringValueIsNotAccepted = "because parameter empty string value is not accepted";

    /// <summary>
    /// The parameter value was empty string or white space and it's not acceptable.
    ///  </summary>
    public const string ParameterWhiteSpaceValueIsNotAccepted = "because parameter white space value is not accepted";

    /// <summary>
    /// The asynchronous method returned null instead of a valid task.
    /// </summary>
    public const string AsyncMethodsMustReturnValidTask = "asynchronous methods must always return valid tasks";
  }
}