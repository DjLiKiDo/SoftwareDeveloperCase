# Input Sanitization Guide

## Overview

This document describes the input sanitization system implemented in the SoftwareDeveloperCase application, designed to prevent security vulnerabilities such as XSS attacks, SQL injection, and other input-based attacks.

## Components

1. **InputSanitizer** - Static utility class with methods for sanitizing different types of inputs
2. **SanitizationBehaviour** - MediatR pipeline behavior that automatically sanitizes incoming request parameters

## Automatic Sanitization

All incoming request parameters are automatically sanitized through the MediatR pipeline using the `SanitizationBehaviour`. This happens before validation and business logic execution, ensuring all data is clean before processing.

The pipeline sanitizes:
- All string properties in request objects
- Nested objects and their properties
- Collections of objects

## Manual Sanitization

For cases where automatic sanitization is not possible (e.g., outside the MediatR pipeline), you can use the `InputSanitizer` class directly:

```csharp
using SoftwareDeveloperCase.Application.Services;

// Basic string sanitization (HTML-encodes and removes control characters)
string sanitized = InputSanitizer.SanitizeString("<script>alert('XSS')</script>");
// Result: "&lt;script&gt;alert(&#39;XSS&#39;)&lt;/script&gt;"

// Plain text sanitization (alphanumeric, spaces, and common punctuation only)
string plainText = InputSanitizer.SanitizePlainText("Hello<script>World</script>");
// Result: "HelloWorld"

// HTML sanitization (removes dangerous tags and attributes while preserving basic formatting)
string safeHtml = InputSanitizer.SanitizeHtml("<p>Hello</p><script>alert('XSS')</script>");
// Result: "<p>Hello</p>"

// Filename sanitization
string safeFilename = InputSanitizer.SanitizeFileName("../malicious.exe");
// Result: "__malicious.exe"

// SQL identifier sanitization
string safeSqlId = InputSanitizer.SanitizeSqlIdentifier("users; DROP TABLE users;");
// Result: "usersDROPTABLEusers"

// Email sanitization
string safeEmail = InputSanitizer.SanitizeEmail("  user@example.com  ");
// Result: "user@example.com"

// URL sanitization
string safeUrl = InputSanitizer.SanitizeUrl("https://example.com");
// Result: "https://example.com"

// Phone number sanitization
string safePhone = InputSanitizer.SanitizePhoneNumber("+1 (123) 456-7890abc");
// Result: "+1 (123) 456-7890"
```

## Best Practices

1. **Automatic vs Manual Sanitization**:
   - Rely on automatic sanitization via the MediatR pipeline for request DTOs
   - Use manual sanitization for:
     - Direct controller inputs (URL parameters, query strings)
     - Form submissions outside of MediatR
     - Free-form text that needs specific sanitization rules

2. **Sanitization Strategy**:
   - For general text input: `SanitizeString`
   - For data to be displayed as HTML: `SanitizeHtml`
   - For data used in SQL identifiers: `SanitizeSqlIdentifier`
   - For filenames: `SanitizeFileName`
   - For plain text where HTML isn't expected: `SanitizePlainText`

3. **Logging**:
   - Always sanitize user input before logging to prevent log injection attacks
   - Example: `_logger.Information("User search: {Query}", InputSanitizer.SanitizeString(userQuery));`

4. **Custom Sanitization**:
   - If you need specialized sanitization beyond what's provided, extend the `InputSanitizer` class with new methods

## Testing

The sanitization system includes unit tests for:
- `InputSanitizerTests` - Tests all sanitization methods with various inputs
- `SanitizationBehaviourTests` - Tests the automatic sanitization pipeline behavior

When implementing new sanitization methods, always add corresponding unit tests.

## Security Considerations

1. Sanitization is not a replacement for parameterized queries in database operations
2. Always validate inputs first, then sanitize
3. Apply the principle of least privilege when passing user input to external systems
4. For complex HTML sanitization needs, consider using a dedicated library like HtmlSanitizer

## Example Implementation

```csharp
[HttpPost]
public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
{
    // The request is automatically sanitized by SanitizationBehaviour,
    // so no manual sanitization is needed here for MediatR requests
    
    var command = _mapper.Map<CreateCommentCommand>(request);
    var result = await _mediator.Send(command);
    
    return Ok(result);
}

[HttpGet("search")]
public async Task<IActionResult> Search([FromQuery] string query)
{
    // Manual sanitization for query parameters not part of MediatR
    var sanitizedQuery = InputSanitizer.SanitizeString(query);
    
    var searchCommand = new SearchCommand { SearchTerm = sanitizedQuery };
    var results = await _mediator.Send(searchCommand);
    
    return Ok(results);
}
```
