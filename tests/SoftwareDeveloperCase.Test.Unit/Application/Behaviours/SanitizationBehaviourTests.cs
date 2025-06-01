using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Xunit;
using SoftwareDeveloperCase.Application.Behaviours;

namespace SoftwareDeveloperCase.Test.Unit.Application.Behaviours;

public class SanitizationBehaviourTests
{
    private class TestRequest : IRequest<string>
    {
        public string? PlainText { get; set; }
        public string? HtmlContent { get; set; }
        public NestedClass? Nested { get; set; }
        
        public class NestedClass
        {
            public string? NestedText { get; set; }
        }
    }

    [Fact]
    public async Task Handle_ShouldSanitizeStringProperties()
    {
        // Arrange
        var request = new TestRequest
        {
            PlainText = "Hello<script>alert('XSS')</script>",
            HtmlContent = "<p onclick=\"alert('evil')\">Click me</p>",
            Nested = new TestRequest.NestedClass
            {
                NestedText = "Nested<script>evil()</script>"
            }
        };

        var sanitizationBehaviour = new SanitizationBehaviour<TestRequest, string>();
        
        var mockNextHandler = new Mock<RequestHandlerDelegate<string>>();
        mockNextHandler.Setup(x => x()).ReturnsAsync("Response");

        // Act
        await sanitizationBehaviour.Handle(request, mockNextHandler.Object, CancellationToken.None);

        // Assert
        Assert.Equal("Hello&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;", request.PlainText);
        Assert.Equal("&lt;p onclick=&quot;alert(&#39;evil&#39;)&quot;&gt;Click me&lt;/p&gt;", request.HtmlContent);
        Assert.Equal("Nested&lt;script&gt;evil()&lt;/script&gt;", request.Nested.NestedText);
        
        mockNextHandler.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullRequest_ShouldProceedToNextHandler()
    {
        // Arrange
        TestRequest? request = null;

        var sanitizationBehaviour = new SanitizationBehaviour<TestRequest, string>();
        
        var mockNextHandler = new Mock<RequestHandlerDelegate<string>>();
        mockNextHandler.Setup(x => x()).ReturnsAsync("Response");

        // Act & Assert (no exceptions should be thrown)
        await sanitizationBehaviour.Handle(request, mockNextHandler.Object, CancellationToken.None);
        
        mockNextHandler.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyStrings_ShouldNotModify()
    {
        // Arrange
        var request = new TestRequest
        {
            PlainText = "",
            HtmlContent = null,
            Nested = new TestRequest.NestedClass
            {
                NestedText = ""
            }
        };

        var sanitizationBehaviour = new SanitizationBehaviour<TestRequest, string>();
        
        var mockNextHandler = new Mock<RequestHandlerDelegate<string>>();
        mockNextHandler.Setup(x => x()).ReturnsAsync("Response");

        // Act
        await sanitizationBehaviour.Handle(request, mockNextHandler.Object, CancellationToken.None);

        // Assert
        Assert.Equal("", request.PlainText);
        Assert.Null(request.HtmlContent);
        Assert.Equal("", request.Nested.NestedText);
        
        mockNextHandler.Verify(x => x(), Times.Once);
    }
}
