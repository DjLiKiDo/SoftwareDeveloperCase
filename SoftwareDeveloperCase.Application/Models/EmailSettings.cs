namespace SoftwareDeveloperCase.Application.Models
{
    public class EmailSettings
    {
        public const string SECTION_NAME = "EmailSettings";

        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; } = 587;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string? FromAddress { get; set; }
        public string? FromName { get; set; }
    }
}
