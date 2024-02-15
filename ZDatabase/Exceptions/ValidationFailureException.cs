using ZDatabase.Validations;

namespace ZDatabase.Exceptions
{
    /// <summary>
    /// Represents when the repository fails during validation.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ValidationFailureException : Exception
    {
        /// <summary>
        /// Gets the validation result.
        /// </summary>
        /// <value>
        /// The validation result.
        /// </value>
        public ValidationResult ValidationResult { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFailureException"/> class.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="message">The message.</param>
        public ValidationFailureException(ValidationResult validationResult, string? message)
            : base(message)
        {
            ValidationResult = validationResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFailureException"/> class.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        public ValidationFailureException(ValidationResult validationResult)
            : this(validationResult, $"Entity has validation problems.")
        {
        }
    }
}