namespace Codelet.Emailing.SendGrid
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Net.Mail;
  using System.Threading.Tasks;
  using global::SendGrid;
  using global::SendGrid.Helpers.Mail;
  using Newtonsoft.Json.Linq;

  /// <summary>
  /// Implements <see cref="IEmailer"/> using SendGrid HTTP API.
  /// </summary>
  public class SendGridHttpEmailer : IEmailer
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SendGridHttpEmailer"/> class.
    /// </summary>
    /// <param name="apiKey">The SendGrid API key.</param>
    public SendGridHttpEmailer(string apiKey)
    {
      this.ApiKey = apiKey;
    }

    private string ApiKey { get; }

    /// <inheritdoc />
    public async Task SendAsync(MailMessage email)
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
        .SendEmailAsync(sendGridEmail)
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