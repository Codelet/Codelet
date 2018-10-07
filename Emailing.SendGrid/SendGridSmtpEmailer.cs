namespace Codelet.Emailing.SendGrid
{
  using System;
  using System.Net;
  using System.Net.Mail;
  using System.Net.Mime;
  using System.Threading.Tasks;

  /// <summary>
  /// Implements <see cref="IEmailer"/> using SendGrid and SMTP protocol.
  /// </summary>
  public class SendGridSmtpEmailer : IEmailer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SendGridSmtpEmailer"/> class.
    /// </summary>
    /// <param name="apiKey">The SendGrid API key.</param>
    public SendGridSmtpEmailer(string apiKey)
    {
      this.Credentials = new NetworkCredential("apiKey", apiKey);
    }

    private ICredentialsByHost Credentials { get; }

    /// <inheritdoc />
    public async Task SendAsync(MailMessage email)
    {
      email = email ?? throw new ArgumentNullException(nameof(email));

      using (var client = new SmtpClient("smtp.sendgrid.net", 587) { Credentials = this.Credentials, EnableSsl = true })
      {
        email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.Body, null, MediaTypeNames.Text.Html));
        await client.SendMailAsync(email).ConfigureAwait(false);
      }
    }
  }
}