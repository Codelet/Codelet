namespace Codelet.Emailing
{
  using System;
  using System.Net.Mail;
  using System.Threading;
  using System.Threading.Tasks;

  /// <summary>
  /// Creates and sends <see cref="MailMessage"/> for the specified <typeparamref name="TEmailModel"/> type.
  /// </summary>
  /// <typeparam name="TEmailModel">The type of the email model.</typeparam>
  public class Emailer<TEmailModel>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Emailer{TEmailModel}" /> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="emailsSender">The emails sender.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="factory" /> == <c>null</c>.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="emailsSender" /> == <c>null</c>.</exception>
    public Emailer(
      IEmailsFactory<TEmailModel> factory,
      EmailsSender emailsSender)
    {
      this.Factory = factory ?? throw new ArgumentNullException(nameof(factory));
      this.EmailsSender = emailsSender ?? throw new ArgumentNullException(nameof(emailsSender));
    }

    private IEmailsFactory<TEmailModel> Factory { get; }

    private EmailsSender EmailsSender { get; }

    /// <summary>
    /// Creates and sends <see cref="MailMessage" /> for the given <paramref name="model" />.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task that represents the process.</returns>
    public virtual async Task SendAsync(TEmailModel model, CancellationToken cancellationToken = default)
    {
      var email = await this.Factory.CreateEmailAsync(model).ConfigureAwait(false);
      await this.EmailsSender(email, cancellationToken).ConfigureAwait(false);
    }
  }
}