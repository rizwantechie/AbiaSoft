namespace AbiaSoft.Services;

public interface IEmailService
{
    Task SendAsync(
        string subject,
        string body,
        string? recipient = null,
        CancellationToken cancellationToken = default);
}
