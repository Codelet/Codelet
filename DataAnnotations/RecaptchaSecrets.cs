namespace Codelet.DataAnnotations
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  /// <summary>
  /// The dictionary of reCAPTCHA secrets.
  /// </summary>
  public sealed class RecaptchaSecrets : ReadOnlyDictionary<string, string>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="RecaptchaSecrets"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary to wrap.</param>
    public RecaptchaSecrets(IDictionary<string, string> dictionary)
      : base(dictionary)
    {
    }
  }
}