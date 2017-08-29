namespace Codelet.DataAnnotations
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
  using Newtonsoft.Json;

  /// <summary>
  /// Validates the reCAPTCHA value.
  /// </summary>
  /// <remarks>
  /// Please make sure that <see cref="ValidationContext.GetService"/>
  /// can return the instance of <see cref="RecaptchaSecrets"/>
  /// that contains the secret for the given <see cref="SiteKey"/>.
  /// Keep in mind it will also try to resolve <see cref="HttpClient"/> as service.
  /// However if it fails it will create new one.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class RecaptchaAttribute
    : ValidationAttribute
  {
    private static readonly Uri RecaptchaApiUrl = new Uri("https://www.google.com/recaptcha/api/siteverify", UriKind.Absolute);

    /// <summary>
    /// Initializes a new instance of the <see cref="RecaptchaAttribute"/> class.
    /// </summary>
    /// <param name="siteKey">The site key.</param>
    public RecaptchaAttribute(string siteKey)
    {
      this.SiteKey = siteKey;
      this.ErrorMessage = "Confirm you're not a robot.";
    }

    /// <summary>
    /// Gets or sets the site key.
    /// </summary>
    public string SiteKey { get; set; }

    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
      => this.IsValidAsync(value, validationContext).Result;

    private async Task<ValidationResult> IsValidAsync(object value, ValidationContext validationContext)
    {
      try
      {
        using (var client = validationContext.GetService<HttpClient>() ?? new HttpClient())
        {
          client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));

          var secret = validationContext.GetService<RecaptchaSecrets>()?[this.SiteKey];

          var postValues = new Dictionary<string, string>
          {
            { "secret", secret },
            { "response", value?.ToString() },
          };

          using (var content = new FormUrlEncodedContent(postValues))
          using (var response = await client.PostAsync(RecaptchaApiUrl, content).ConfigureAwait(false))
          {
            var responseContentString = await response
              .EnsureSuccessStatusCode()
              .Content
              .ReadAsStringAsync()
              .ConfigureAwait(false);

            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(responseContentString);

            if (captchaResponse.Success)
            {
              return ValidationResult.Success;
            }
          }
        }
      }
      catch (HttpRequestException)
      {
      }

      return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
    }

    private sealed class CaptchaResponse
    {
      [JsonProperty("success")]
      public bool Success { get; set; }

      [JsonProperty("error-codes")]
      public List<string> ErrorCodes { get; set; }
    }
  }
}