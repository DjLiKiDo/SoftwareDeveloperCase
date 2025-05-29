namespace SoftwareDeveloperCase.Application.Models
{
    /// <summary>
    /// Represents an email message
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Gets or sets the recipient email address
        /// </summary>
        public string? To { get; set; }
        
        /// <summary>
        /// Gets or sets the email subject
        /// </summary>
        public string? Subject { get; set; }
        
        /// <summary>
        /// Gets or sets the email body content
        /// </summary>
        public string? Body { get; set; }
    }
}
