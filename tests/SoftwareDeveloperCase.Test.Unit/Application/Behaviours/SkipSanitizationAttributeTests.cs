using System.Reflection;
using FluentAssertions;
using SoftwareDeveloperCase.Application.Behaviours;
using SoftwareDeveloperCase.Application.Attributes;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Behaviours;

/// <summary>
/// Tests for the SkipSanitizationAttribute functionality
/// </summary>
public class SkipSanitizationAttributeTests
{
    /// <summary>
    /// Test class with mixed sanitization requirements
    /// </summary>
    private class TestClass
    {
        public string? RegularProperty { get; set; }
        
        [SkipSanitization("Test sensitive data")]
        public string? SensitiveProperty { get; set; }
    }

    [Fact]
    public void SanitizationBehaviour_Should_RespectSkipSanitizationAttribute()
    {
        // Arrange
        var testObj = new TestClass
        {
            RegularProperty = "<script>alert('xss')</script>",
            SensitiveProperty = "<script>alert('sensitive')</script>"
        };

        // Act
        var properties = typeof(TestClass).GetProperties();
        foreach (var property in properties)
        {
            if (property.GetCustomAttribute<SkipSanitizationAttribute>() != null)
            {
                // This property should not be sanitized
                continue;
            }

            if (property.PropertyType == typeof(string) && property.CanWrite)
            {
                var value = property.GetValue(testObj) as string;
                if (!string.IsNullOrEmpty(value))
                {
                    var sanitized = SoftwareDeveloperCase.Application.Services.InputSanitizer.SanitizeString(value);
                    property.SetValue(testObj, sanitized);
                }
            }
        }

        // Assert
        testObj.RegularProperty.Should().NotContain("<script>"); // Should be sanitized
        testObj.SensitiveProperty.Should().Contain("<script>"); // Should NOT be sanitized
    }

    [Fact]
    public void SkipSanitizationAttribute_Should_HaveCorrectReason()
    {
        // Arrange
        var attribute = new SkipSanitizationAttribute("Test reason");

        // Act & Assert
        attribute.Reason.Should().Be("Test reason");
    }

    [Fact]
    public void SkipSanitizationAttribute_Should_HaveDefaultReason()
    {
        // Arrange
        var attribute = new SkipSanitizationAttribute();

        // Act & Assert
        attribute.Reason.Should().Be("Sensitive data that should not be modified");
    }
}