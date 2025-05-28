namespace SoftwareDeveloperCase.Application.Models
{
    public class EmailSettings
    {
        public const string SECTION_NAME = "EmailSettings";

        public string? ApiKey { get; set; }
        public string? FromAddress { get; set; }
        public string? FromName { get; set; }
    }
}
