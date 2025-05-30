using System.ComponentModel.DataAnnotations;

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
    [Required]
    public string? SmtpServer { get; set; }

    /// <summary>
    /// Gets or sets the SMTP server port
    /// </summary>
    [Range(1, 65535)]
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
    [Required]
    [EmailAddress]
    public string? FromAddress { get; set; }

    /// <summary>
    /// Gets or sets the sender display name
    /// </summary>
    [Required]
    public string? FromName { get; set; }
}
