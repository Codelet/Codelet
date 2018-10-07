namespace Codelet.Emailing
{
  using System.Net.Mail;
  using System.Threading.Tasks;

  /// <summary>
  /// Creates <see cref="MailMessage"/> for the specified <typeparamref name="TEmailModel"/> type.
  /// </summary>
  /// <typeparam name="TEmailModel">The type of the email model.</typeparam>
  public interface IEmailsFactory<in TEmailModel>
  {
    /// <summary>
    /// Creates the <see cref="MailMessage"/> for the given <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The task that represents the process.</returns>
    Task<MailMessage> CreateEmailAsync(TEmailModel model);
  }
}