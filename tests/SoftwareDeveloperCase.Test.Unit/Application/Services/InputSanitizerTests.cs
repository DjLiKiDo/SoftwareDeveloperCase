using System;
using Xunit;
using SoftwareDeveloperCase.Application.Services;

namespace SoftwareDeveloperCase.Test.Unit.Application.Services;

public class InputSanitizerTests
{
    [Theory]
    [InlineData("<script>alert('XSS')</script>Hello", "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;Hello")]
    [InlineData("Normal text", "Normal text")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeString_ShouldHandleVariousInputs(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeString(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello123!@#", "Hello123!@#")]
    [InlineData("Hello<script>", "Hello")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizePlainText_ShouldRemoveDangerousCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizePlainText(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("file.txt", "file.txt")]
    [InlineData("../file.txt", "__file.txt")]
    [InlineData("file?.txt", "file_.txt")]
    [InlineData("CON.txt", "_CON.txt")]
    [InlineData("file<>|.txt", "file___.txt")]
    [InlineData("...file.txt", "_file.txt")]
    [InlineData("file.txt   ", "file.txt")]
    [InlineData("file/path.txt", "file_path.txt")]
    [InlineData("file\\path.txt", "file_path.txt")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeFileName_ShouldRemoveDangerousCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeFileName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("<p>Hello</p><script>alert('XSS')</script>", "<p>Hello</p>")]
    [InlineData("<p onclick=\"alert('XSS')\">Click me</p>", "<p>Click me</p>")]
    [InlineData("<iframe src=\"evil.com\"></iframe>", "")]
    [InlineData("<a href=\"javascript:alert('XSS')\">Click</a>", "<a>Click</a>")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeHtml_ShouldRemoveDangerousElements(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeHtml(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("column_name", "column_name")]
    [InlineData("column;DROP TABLE users", "columnDROPTABLEusers")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeSqlIdentifier_ShouldRemoveDangerousCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeSqlIdentifier(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("test@example.com", "test@example.com")]
    [InlineData("test.user+tag@example.co.uk", "test.user+tag@example.co.uk")]
    [InlineData("invalid email", null)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeEmail_ShouldHandleVariousInputs(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeEmail(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("http://example.com", "http://example.com")]
    [InlineData("https://example.com/path?query=value", "https://example.com/path?query=value")]
    [InlineData("javascript:alert('XSS')", null)]
    [InlineData("not a url", null)]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeUrl_ShouldHandleVariousInputs(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeUrl(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("+1 (123) 456-7890", "+1 (123) 456-7890")]
    [InlineData("123-456-7890", "123-456-7890")]
    [InlineData("123.456.7890", "123.456.7890")]
    [InlineData("123ABC456", "123456")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizePhoneNumber_ShouldHandleVariousInputs(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizePhoneNumber(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Normal log message", "Normal log message")]
    [InlineData("Message with\nnewline", "Message withnewline")]
    [InlineData("Message with\r\nCRLF", "Message withCRLF")]
    [InlineData("Message with\ttab", "Message withtab")]
    [InlineData("Message  with   multiple    spaces", "Message with multiple spaces")]
    [InlineData("<script>alert('XSS')</script>", "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;")]
    [InlineData("Log injection\nFAKE LOG: User admin deleted", "Log injectionFAKE LOG: User admin deleted")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeForLogging_ShouldRemoveLogInjectionVectors(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeForLogging(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("(cn=user)", "\\28cn=user\\29")]
    [InlineData("uid=admin*", "uid=admin\\2a")]
    [InlineData("normal_user", "normal_user")]
    [InlineData("user\\name", "user\\5cname")]
    [InlineData("special(*)chars", "special\\28\\2a\\29chars")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeLdap_ShouldEscapeLdapSpecialCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeLdap(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{\"key\": \"value\"}", "{\\\"key\\\": \\\"value\\\"}")]
    [InlineData("Normal string", "Normal string")]
    [InlineData("String with \"quotes\"", "String with \\\"quotes\\\"")]
    [InlineData("String with \\ backslash", "String with \\\\ backslash")]
    [InlineData("String with\ncontrol chars", "String with\\ncontrol chars")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeJson_ShouldEscapeJsonSpecialCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeJson(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("user_name", "user_name")]
    [InlineData("$where: function() { return true; }", ": ) { return true; }")]
    [InlineData("{ $ne: null }", "{ : null }")]
    [InlineData("eval(malicious_code)", "malicious_code)")]
    [InlineData("$regex", "")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeNoSql_ShouldRemoveNoSqlInjectionPatterns(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeNoSql(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("normal_command", "normal_command")]
    [InlineData("rm -rf /", "-rf /")]
    [InlineData("echo hello; rm file", "echo hello file")]
    [InlineData("ls | grep test", "ls  grep test")]
    [InlineData("wget http://evil.com", "wget http://evil.com")]
    [InlineData("curl -X POST", "curl -X POST")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeCommand_ShouldRemoveCommandInjectionCharacters(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeCommand(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello World", "Hello World")]
    [InlineData("{{user.name}}", "")]
    [InlineData("<%= evil_code %>", "")]
    [InlineData("${dangerous}", "")]
    [InlineData("#{ruby_code}", "")]
    [InlineData("Normal text with {{var}}", "Normal text with ")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void SanitizeTemplate_ShouldRemoveTemplateInjectionPatterns(string? input, string? expected)
    {
        // Act
        var result = InputSanitizer.SanitizeTemplate(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
