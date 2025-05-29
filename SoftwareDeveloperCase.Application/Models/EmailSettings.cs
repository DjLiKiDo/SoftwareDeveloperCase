namespace SoftwareDeveloperCase.Application.Models;

/// <summary>
/// Configuration settings for email service
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// The configuration section name for email settings
    /// </summary>
    public const string SECTION_NAME = "EmailSettings";

    /// <summary>
    /// Gets or sets the SMTP server address
    /// </summary>
    public string? SmtpServer { get; set; }

    /// <summary>
    /// Gets or sets the SMTP server port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Gets or sets the username for SMTP authentication
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for SMTP authentication
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets whether SSL is enabled for SMTP connection
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets the sender email address
    /// </summary>
    public string? FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the sender display name
    /// </summary>
    public string? FromName { get; set; }
}
