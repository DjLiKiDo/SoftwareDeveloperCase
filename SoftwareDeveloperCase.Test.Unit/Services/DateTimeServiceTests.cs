using FluentAssertions;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Infrastructure.Services;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Services;

public class DateTimeServiceTests
{
    [Fact]
    public void DateTimeService_ShouldImplementIDateTimeService()
    {
        // Arrange & Act
        var dateTimeServiceType = typeof(DateTimeService);
        var interfaceType = typeof(IDateTimeService);

        // Assert
        interfaceType.IsAssignableFrom(dateTimeServiceType).Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithoutParameters()
    {
        // Arrange & Act
        var dateTimeService = new DateTimeService();

        // Assert
        dateTimeService.Should().NotBeNull();
    }

    [Fact]
    public void Now_ShouldReturnUtcDateTime()
    {
        // Arrange
        var dateTimeService = new DateTimeService();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = dateTimeService.Now;
        var afterCall = DateTime.UtcNow;

        // Assert
        result.Should().BeOnOrAfter(beforeCall);
        result.Should().BeOnOrBefore(afterCall);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void Now_ShouldReturnDifferentValuesOnConsecutiveCalls()
    {
        // Arrange
        var dateTimeService = new DateTimeService();

        // Act
        var firstCall = dateTimeService.Now;
        Thread.Sleep(1); // Ensure some time passes
        var secondCall = dateTimeService.Now;

        // Assert
        secondCall.Should().BeOnOrAfter(firstCall);
    }

    [Fact]
    public void Now_ShouldBeConsistentWithSystemUtcNow()
    {
        // Arrange
        var dateTimeService = new DateTimeService();
        var systemUtcNow = DateTime.UtcNow;

        // Act
        var serviceNow = dateTimeService.Now;

        // Assert
        var timeDifference = Math.Abs((serviceNow - systemUtcNow).TotalMilliseconds);
        timeDifference.Should().BeLessThan(1000); // Should be within 1 second
    }

    [Fact]
    public void IDateTimeService_ShouldHaveNowProperty()
    {
        // Arrange
        var dateTimeServiceType = typeof(IDateTimeService);

        // Act
        var nowProperty = dateTimeServiceType.GetProperty("Now");

        // Assert
        nowProperty.Should().NotBeNull();
        nowProperty!.PropertyType.Should().Be(typeof(DateTime));
        nowProperty.CanRead.Should().BeTrue();
        nowProperty.CanWrite.Should().BeFalse();
    }
}