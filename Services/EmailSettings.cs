namespace AbiaSoft.Services;

public sealed class EmailSettings
{
    public const string SectionName = "EmailSettings";

    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
