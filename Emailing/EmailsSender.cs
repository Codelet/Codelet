namespace Codelet.Emailing
{
  using System.Net.Mail;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Sends the <paramref name="email"/>.
  /// </summary>
  /// <param name="email">The email.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The task that represents the process.</returns>
  public delegate Task EmailsSender(MailMessage email, CancellationToken cancellationToken = default);
}