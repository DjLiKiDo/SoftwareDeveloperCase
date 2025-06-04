using FluentAssertions;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("test+tag@example.org")]
    [InlineData("user123@test-domain.com")]
    [InlineData("Test@Example.COM")]
    public void Constructor_ValidEmail_ShouldCreateSuccessfully(string emailAddress)
    {
        // Arrange & Act
        var email = new Email(emailAddress);

        // Assert
        email.Value.Should().Be(emailAddress.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_NullOrEmptyEmail_ShouldThrowArgumentException(string? emailAddress)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Email(emailAddress!));
        exception.Message.Should().Contain("Email cannot be null or empty");
        exception.ParamName.Should().Be("value");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    [InlineData("user.domain.com")]
    [InlineData("user@domain.")]
    public void Constructor_InvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Email(invalidEmail));
        exception.Message.Should().Contain("Invalid email format");
        exception.ParamName.Should().Be("value");
    }

    [Fact]
    public void Domain_ValidEmail_ShouldReturnCorrectDomain()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        var domain = email.Domain;

        // Assert
        domain.Should().Be("example.com");
    }

    [Fact]
    public void LocalPart_ValidEmail_ShouldReturnCorrectLocalPart()
    {
        // Arrange
        var email = new Email("user.name@example.com");

        // Act
        var localPart = email.LocalPart;

        // Assert
        localPart.Should().Be("user.name");
    }

    [Fact]
    public void ImplicitConversion_EmailToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        string emailString = email;

        // Assert
        emailString.Should().Be("test@example.com");
    }

    [Fact]
    public void ExplicitConversion_StringToEmail_ShouldCreateEmail()
    {
        // Arrange
        const string emailString = "test@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        email.Value.Should().Be(emailString);
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void Equals_SameEmailValues_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Should().Be(email2);
        email1.Equals(email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentEmailValues_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act & Assert
        email1.Should().NotBe(email2);
        email1.Equals(email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameEmailDifferentCase_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("Test@Example.COM");
        var email2 = new Email("test@example.com");

        // Act & Assert
        email1.Should().Be(email2);
        email1.Equals(email2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameEmailValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = new Email("test@example.com");
        var email2 = new Email("test@example.com");

        // Act
        var hash1 = email1.GetHashCode();
        var hash2 = email2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_DifferentEmailValues_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var email1 = new Email("test1@example.com");
        var email2 = new Email("test2@example.com");

        // Act
        var hash1 = email1.GetHashCode();
        var hash2 = email2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("test@example.com");

        // Act & Assert
        email.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("test@example.com");
        var otherObject = "test@example.com";

        // Act & Assert
        email.Equals(otherObject).Should().BeFalse();
    }

    [Fact]
    public void EmailNormalization_UpperCaseInput_ShouldBeLowerCase()
    {
        // Arrange & Act
        var email = new Email("USER@EXAMPLE.COM");

        // Assert
        email.Value.Should().Be("user@example.com");
        email.Domain.Should().Be("example.com");
        email.LocalPart.Should().Be("user");
    }
}
