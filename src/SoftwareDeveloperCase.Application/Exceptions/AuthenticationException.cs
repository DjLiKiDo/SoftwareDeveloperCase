namespace SoftwareDeveloperCase.Application.Exceptions;

/// <summary>
/// Exception thrown when authentication fails
/// </summary>
public class AuthenticationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class
    /// </summary>
    public AuthenticationException() : base("Authentication failed")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message
    /// </summary>
    /// <param name="message">The error message</param>
    public AuthenticationException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationException"/> class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
