using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AbiaSoft.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<EmailSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(
        string subject,
        string body,
        string? recipient = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Password))
        {
            _logger.LogError("Cannot send email because the SMTP password is not configured.");
            throw new InvalidOperationException("Email password is not configured.");
        }

        var recipientAddress = recipient ?? _settings.RecipientEmail;
        if (string.IsNullOrWhiteSpace(recipientAddress))
        {
            _logger.LogError("Cannot send email because no recipient is configured or provided.");
            throw new InvalidOperationException("Email recipient is not configured.");
        }

        _logger.LogInformation(
            "Sending email through SMTP server {SmtpServer}:{SmtpPort}.",
            _settings.SmtpServer,
            _settings.Port);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipientAddress));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };

       using var smtp = new SmtpClient();

// Diagnostic workaround for the incomplete certificate revocation check.
smtp.CheckCertificateRevocation = false;

        try
        {
            _logger.LogDebug(
                "Connecting to SMTP server {SmtpServer}:{SmtpPort} with STARTTLS.",
                _settings.SmtpServer,
                _settings.Port);
            await smtp.ConnectAsync(
                _settings.SmtpServer,
                _settings.Port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            _logger.LogDebug("Authenticating with SMTP server {SmtpServer}.", _settings.SmtpServer);
            await smtp.AuthenticateAsync(
                _settings.SenderEmail,
                _settings.Password,
                cancellationToken);

            _logger.LogDebug("Submitting email through SMTP server {SmtpServer}.", _settings.SmtpServer);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully through SMTP server {SmtpServer}:{SmtpPort}.",
                _settings.SmtpServer,
                _settings.Port);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Email sending was cancelled through SMTP server {SmtpServer}:{SmtpPort}.",
                _settings.SmtpServer,
                _settings.Port);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email through SMTP server {SmtpServer}:{SmtpPort}.",
                _settings.SmtpServer,
                _settings.Port);
            throw;
        }
    }
}
