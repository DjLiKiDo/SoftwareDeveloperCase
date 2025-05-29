using FluentValidation.Results;

namespace SoftwareDeveloperCase.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when validation failures occur
    /// </summary>
    public class ValidationException : ApplicationException
    {
        /// <summary>
        /// Gets the validation errors grouped by property name
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the ValidationException class
        /// </summary>
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Initializes a new instance of the ValidationException class with validation failures
        /// </summary>
        /// <param name="failures">The collection of validation failures</param>
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
