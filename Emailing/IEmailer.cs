namespace Codelet.Emailing
{
  using System.Net.Mail;
  using System.Threading.Tasks;

  /// <summary>
  /// Sends the <see cref="MailMessage"/>.
  /// </summary>
  public interface IEmailer
  {
    /// <summary>
    /// Sends the <paramref name="email"/>.
    /// </summary>
    /// <param name="email">The email.</param>
    /// <returns>The task that represents the process.</returns>
    Task SendAsync(MailMessage email);
  }
}