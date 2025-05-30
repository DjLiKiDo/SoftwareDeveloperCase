using System.ComponentModel.DataAnnotations;

namespace SoftwareDeveloperCase.Application.Models;

/// <summary>
/// Configuration settings for database connection and provider
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// The configuration section name for database settings
    /// </summary>
    public const string SECTION_NAME = "DatabaseSettings";

    /// <summary>
    /// Gets or sets whether to use in-memory database for testing/development
    /// </summary>
    public bool UseInMemoryDatabase { get; set; } = true;

    /// <summary>
    /// Gets or sets the database provider type
    /// </summary>
    [Required]
    public string DatabaseProvider { get; set; } = "InMemory";

    /// <summary>
    /// Gets or sets the connection string for the database
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the command timeout in seconds
    /// </summary>
    [Range(1, 3600)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets whether to enable detailed errors in development
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable sensitive data logging
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;
}