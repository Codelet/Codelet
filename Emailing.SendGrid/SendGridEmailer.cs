namespace Codelet.Emailing.SendGrid
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Net.Mail;
  using System.Net.Mime;
  using System.Threading;
  using System.Threading.Tasks;
  using global::SendGrid;
  using global::SendGrid.Helpers.Mail;
  using Newtonsoft.Json.Linq;

  /// <summary>
  /// Sends <see cref="MailMessage"/> using SendGrid over SMTP or HTTP protocols.
  /// </summary>
  public class SendGridEmailer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SendGridEmailer"/> class.
    /// </summary>
    /// <param name="apiKey">The SendGrid API key.</param>
    public SendGridEmailer(string apiKey)
    {
      this.ApiKey = apiKey;
    }

    private string ApiKey { get; }

    /// <summary>
    /// Sends the <paramref name="email"/> over SMTP protocol.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    public async Task SendOverSmtpAsync(MailMessage email, CancellationToken cancellationToken = default)
    {
      email = email ?? throw new ArgumentNullException(nameof(email));

      using (var client = new SmtpClient("smtp.sendgrid.net", 587)
      {
        Credentials = new NetworkCredential("apiKey", this.ApiKey),
        EnableSsl = true,
      })
      {
        email.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(email.Body, null, MediaTypeNames.Text.Html));
        await client.SendMailAsync(email).ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Sends the <paramref name="email"/> over HTTP protocol.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    public async Task SendOverHttpAsync(MailMessage email, CancellationToken cancellationToken = default)
    {
      email = email ?? throw new ArgumentNullException(nameof(email));

      var sendGridEmail = new SendGridMessage
      {
        Subject = email.Subject,
        From = ToSendGridEmailAddress(email.From) ?? ToSendGridEmailAddress(email.Sender),
        ReplyTo = ToSendGridEmailAddress(email.ReplyToList.FirstOrDefault()),
        Headers = email.Headers.AllKeys.ToDictionary(key => key, key => email.Headers[key]),
        Personalizations = new List<Personalization>
        {
          new Personalization
          {
            Tos = email.To.Select(ToSendGridEmailAddress).ToList(),
            Ccs = email.CC.Any() ? email.CC.Select(ToSendGridEmailAddress).ToList() : null,
            Bccs = email.Bcc.Any() ? email.Bcc.Select(ToSendGridEmailAddress).ToList() : null,
          },
        },

        PlainTextContent = email.Body,
        HtmlContent = email.Body,

        Attachments = email.Attachments.Any()
          ? email
            .Attachments
            .Select(attachment => new global::SendGrid.Helpers.Mail.Attachment
            {
              Type = attachment.ContentType.ToString(),
              ContentId = attachment.ContentId,
              Disposition = attachment.ContentDisposition.ToString(),
              Filename = attachment.Name,
              Content = ToBase64String(attachment.ContentStream),
            })
            .ToList()
          : null,
      };

      var response = await new SendGridClient(this.ApiKey)
        .SendEmailAsync(sendGridEmail, cancellationToken)
        .ConfigureAwait(false);

      if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
      {
        throw new HttpRequestException(JObject
          .Parse(await response.Body.ReadAsStringAsync().ConfigureAwait(false))
          .ToString());
      }
    }

    private static EmailAddress ToSendGridEmailAddress(MailAddress address)
      => address != null ? new EmailAddress(address.Address, address.DisplayName) : null;

    private static string ToBase64String(Stream stream)
    {
      var buffer = new byte[stream.Length];
      stream.ReadAsync(buffer, 0, buffer.Length);

      return Convert.ToBase64String(buffer);
    }
  }
}