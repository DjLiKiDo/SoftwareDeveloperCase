using FluentValidation;
using FluentValidation.TestHelper;
using SoftwareDeveloperCase.Application.Validators;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Application.Validators;

public class PasswordComplexityValidatorTests
{
    private readonly TestValidator _validator;

    public PasswordComplexityValidatorTests()
    {
        _validator = new TestValidator();
    }

    [Theory]
    [InlineData("Tr0ub4dor&3!", true)]  // Valid password
    [InlineData("M0rning$un9!", true)]  // Valid password
    [InlineData("C0ffee#B34n7", true)]  // Valid password
    [InlineData("", false)]  // Empty
    [InlineData(null, false)]  // Null
    [InlineData("short1!", false)]  // Too short
    [InlineData("UPPERCASE123!", false)]  // No lowercase
    [InlineData("lowercase123!", false)]  // No uppercase
    [InlineData("NoNumbers!", false)]  // No numbers
    [InlineData("NoSpecialChars123", false)]  // No special characters
    [InlineData("password", false)]  // Common password
    [InlineData("MyPassword123!", false)]  // Contains "password"
    [InlineData("123456789A!", false)]  // Common password
    [InlineData("QwertySecure1!", false)]  // Contains "qwerty"
    public void PasswordComplexity_ShouldValidateCorrectly(string? password, bool shouldBeValid)
    {
        // Act
        var result = _validator.TestValidate(new TestModel { Password = password });

        // Assert
        if (shouldBeValid)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }
    }

    [Fact]
    public void PasswordComplexity_WithTooLongPassword_ShouldBeInvalid()
    {
        // Arrange
        var longPassword = new string('A', 130) + "a1!"; // 133 characters, exceeds 128 limit

        // Act
        var result = _validator.TestValidate(new TestModel { Password = longPassword });

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must not exceed 128 characters");
    }

    [Theory]
    [InlineData("Password", "Password is too common, please choose a stronger password")]
    [InlineData("123456", "Password is too common, please choose a stronger password")]
    [InlineData("admin", "Password is too common, please choose a stronger password")]
    [InlineData("short", "Password must be at least 8 characters long")]
    [InlineData("nouppercase123!", "Password must contain at least one uppercase letter")]
    [InlineData("NOLOWERCASE123!", "Password must contain at least one lowercase letter")]
    [InlineData("NoNumbers!", "Password must contain at least one number")]
    [InlineData("NoSpecialChars123", "Password must contain at least one special character")]
    public void PasswordComplexity_WithSpecificErrors_ShouldReturnExpectedMessage(string password, string expectedMessage)
    {
        // Act
        var result = _validator.TestValidate(new TestModel { Password = password });

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(expectedMessage);
    }

    [Fact]
    public void PasswordComplexity_WithValidComplexPassword_ShouldPass()
    {
        // Arrange
        var validPassword = "Tr0ub4dor&3!";

        // Act
        var result = _validator.TestValidate(new TestModel { Password = validPassword });

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordComplexity_CaseInsensitiveCommonPasswordCheck_ShouldFail()
    {
        // Arrange
        var passwords = new[] { "PASSWORD", "Password123!", "pAsSwOrD1!", "LETMEIN7@", "letmein9!" };

        foreach (var password in passwords)
        {
            // Act
            var result = _validator.TestValidate(new TestModel { Password = password });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password is too common, please choose a stronger password");
        }
    }

    private class TestValidator : AbstractValidator<TestModel>
    {
        public TestValidator()
        {
            RuleFor(x => x.Password).PasswordComplexity();
        }
    }

    private class TestModel
    {
        public string? Password { get; set; }
    }
}
