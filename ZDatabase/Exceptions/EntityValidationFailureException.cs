using ZDatabase.Validations;

namespace ZDatabase.Exceptions
{
    /// <summary>
    /// Represents when the repository fails during an entity validation.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="ZDatabase.Exceptions.ValidationFailureException" />
    public class EntityValidationFailureException<TKey> : ValidationFailureException
        where TKey : struct
    {
        /// <summary>
        /// Gets the entity key.
        /// </summary>
        /// <value>
        /// The entity key.
        /// </value>
        public TKey EntityKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidationFailureException{TKey}"/> class.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <param name="key">The key.</param>
        /// <param name="validationResult">The validation result.</param>
        public EntityValidationFailureException(string entityName, TKey key, ValidationResult validationResult)
            : base(validationResult, $"Entity '{entityName} ({key})' has validation problems.")
        {
            EntityKey = key;
        }
    }
}