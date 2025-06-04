using FluentAssertions;
using SoftwareDeveloperCase.Domain.ValueObjects;
using Xunit;

namespace SoftwareDeveloperCase.Test.Unit.Domain.ValueObjects;

public class TaskHierarchyTests
{
    [Theory]
    [InlineData(0, "1", 1)]
    [InlineData(1, "1.2", 2)]
    [InlineData(2, "1.2.3", 3)]
    public void Constructor_ValidParameters_ShouldCreateSuccessfully(int level, string path, int order)
    {
        // Arrange & Act
        var hierarchy = new TaskHierarchy(level, path, order);

        // Assert
        hierarchy.Level.Should().Be(level);
        hierarchy.Path.Should().Be(path);
        hierarchy.Order.Should().Be(order);
    }

    [Theory]
    [InlineData(-1, "1", 1)]
    [InlineData(-5, "1.2", 2)]
    public void Constructor_NegativeLevel_ShouldThrowArgumentException(int invalidLevel, string path, int order)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new TaskHierarchy(invalidLevel, path, order));
        exception.Message.Should().Contain("Level cannot be negative");
        exception.ParamName.Should().Be("level");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_InvalidPath_ShouldThrowArgumentException(string? invalidPath)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new TaskHierarchy(0, invalidPath!, 1));
        exception.Message.Should().Contain("Path cannot be null or empty");
        exception.ParamName.Should().Be("path");
    }

    [Theory]
    [InlineData(0, "1", -1)]
    [InlineData(1, "1.2", -5)]
    public void Constructor_NegativeOrder_ShouldThrowArgumentException(int level, string path, int invalidOrder)
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new TaskHierarchy(level, path, invalidOrder));
        exception.Message.Should().Contain("Order cannot be negative");
        exception.ParamName.Should().Be("order");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateRoot_ValidOrder_ShouldCreateRootHierarchy(int order)
    {
        // Arrange & Act
        var hierarchy = TaskHierarchy.CreateRoot(order);

        // Assert
        hierarchy.Level.Should().Be(0);
        hierarchy.Path.Should().Be(order.ToString());
        hierarchy.Order.Should().Be(order);
        hierarchy.IsRoot.Should().BeTrue();
    }

    [Fact]
    public void CreateChild_ValidParentAndOrder_ShouldCreateChildHierarchy()
    {
        // Arrange
        var parentHierarchy = TaskHierarchy.CreateRoot(1);
        const int childOrder = 2;

        // Act
        var childHierarchy = TaskHierarchy.CreateChild(parentHierarchy, childOrder);

        // Assert
        childHierarchy.Level.Should().Be(1);
        childHierarchy.Path.Should().Be("1.2");
        childHierarchy.Order.Should().Be(childOrder);
        childHierarchy.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void CreateChild_MultiLevel_ShouldCreateCorrectHierarchy()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);
        var childHierarchy = TaskHierarchy.CreateChild(rootHierarchy, 2);

        // Act
        var grandChildHierarchy = TaskHierarchy.CreateChild(childHierarchy, 3);

        // Assert
        grandChildHierarchy.Level.Should().Be(2);
        grandChildHierarchy.Path.Should().Be("1.2.3");
        grandChildHierarchy.Order.Should().Be(3);
        grandChildHierarchy.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void GetParentPath_RootTask_ShouldReturnNull()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);

        // Act
        var parentPath = rootHierarchy.GetParentPath();

        // Assert
        parentPath.Should().BeNull();
    }

    [Fact]
    public void GetParentPath_ChildTask_ShouldReturnParentPath()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);
        var childHierarchy = TaskHierarchy.CreateChild(rootHierarchy, 2);

        // Act
        var parentPath = childHierarchy.GetParentPath();

        // Assert
        parentPath.Should().Be("1");
    }

    [Fact]
    public void GetParentPath_GrandChildTask_ShouldReturnParentPath()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);
        var childHierarchy = TaskHierarchy.CreateChild(rootHierarchy, 2);
        var grandChildHierarchy = TaskHierarchy.CreateChild(childHierarchy, 3);

        // Act
        var parentPath = grandChildHierarchy.GetParentPath();

        // Assert
        parentPath.Should().Be("1.2");
    }

    [Fact]
    public void IsRoot_RootTask_ShouldReturnTrue()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);

        // Act & Assert
        rootHierarchy.IsRoot.Should().BeTrue();
    }

    [Fact]
    public void IsRoot_ChildTask_ShouldReturnFalse()
    {
        // Arrange
        var rootHierarchy = TaskHierarchy.CreateRoot(1);
        var childHierarchy = TaskHierarchy.CreateChild(rootHierarchy, 2);

        // Act & Assert
        childHierarchy.IsRoot.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnPath()
    {
        // Arrange
        var hierarchy = new TaskHierarchy(1, "1.2.3", 3);

        // Act
        var result = hierarchy.ToString();

        // Assert
        result.Should().Be("1.2.3");
    }

    [Fact]
    public void Equals_SameHierarchyValues_ShouldReturnTrue()
    {
        // Arrange
        var hierarchy1 = new TaskHierarchy(1, "1.2", 2);
        var hierarchy2 = new TaskHierarchy(1, "1.2", 2);

        // Act & Assert
        hierarchy1.Should().Be(hierarchy2);
        hierarchy1.Equals(hierarchy2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentHierarchyValues_ShouldReturnFalse()
    {
        // Arrange
        var hierarchy1 = new TaskHierarchy(1, "1.2", 2);
        var hierarchy2 = new TaskHierarchy(1, "1.3", 3);

        // Act & Assert
        hierarchy1.Should().NotBe(hierarchy2);
        hierarchy1.Equals(hierarchy2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameHierarchyValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var hierarchy1 = new TaskHierarchy(1, "1.2", 2);
        var hierarchy2 = new TaskHierarchy(1, "1.2", 2);

        // Act
        var hash1 = hierarchy1.GetHashCode();
        var hash2 = hierarchy2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_DifferentHierarchyValues_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var hierarchy1 = new TaskHierarchy(1, "1.2", 2);
        var hierarchy2 = new TaskHierarchy(1, "1.3", 3);

        // Act
        var hash1 = hierarchy1.GetHashCode();
        var hash2 = hierarchy2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var hierarchy = new TaskHierarchy(1, "1.2", 2);

        // Act & Assert
        hierarchy.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // Arrange
        var hierarchy = new TaskHierarchy(1, "1.2", 2);
        var otherObject = "1.2";

        // Act & Assert
        hierarchy.Equals(otherObject).Should().BeFalse();
    }

    [Fact]
    public void ComplexHierarchy_FourLevelsDeep_ShouldWorkCorrectly()
    {
        // Arrange
        var level0 = TaskHierarchy.CreateRoot(1);
        var level1 = TaskHierarchy.CreateChild(level0, 2);
        var level2 = TaskHierarchy.CreateChild(level1, 3);
        var level3 = TaskHierarchy.CreateChild(level2, 4);

        // Act & Assert
        level0.Level.Should().Be(0);
        level0.Path.Should().Be("1");
        level0.IsRoot.Should().BeTrue();

        level1.Level.Should().Be(1);
        level1.Path.Should().Be("1.2");
        level1.IsRoot.Should().BeFalse();
        level1.GetParentPath().Should().Be("1");

        level2.Level.Should().Be(2);
        level2.Path.Should().Be("1.2.3");
        level2.IsRoot.Should().BeFalse();
        level2.GetParentPath().Should().Be("1.2");

        level3.Level.Should().Be(3);
        level3.Path.Should().Be("1.2.3.4");
        level3.IsRoot.Should().BeFalse();
        level3.GetParentPath().Should().Be("1.2.3");
    }
}