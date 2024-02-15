using ZDatabase.Entities;
using ZDatabase.Exceptions;

namespace ZDatabase.Validations
{
    /// <summary>
    /// The validation result to handle the validation.
    /// </summary>
    public class ValidationResult
    {
        #region Variables
        #endregion

        #region Properties        
        /// <summary>
        /// Gets or sets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get => Errors.Count > 0;
        }
        #endregion

        #region Constructor
        #endregion

        #region Public methods        
        /// <summary>
        /// Sets the error.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="message">The message.</param>
        public void SetError(string key, string message)
        {
            if (!Errors.ContainsKey(key))
            {
                Errors.Add(key, new[] { message });
            }
            else
            {
                IList<string> errors = Errors[key].ToList();
                errors.Add(message);
                Errors[key] = errors.ToArray();
            }
        }

        /// <summary>
        /// Validates and throws <see cref="ZDatabase.Exceptions.ValidationFailureException"/> when the entity has errors.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void ValidateEntityErrors<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            if (HasErrors)
            {
                throw new EntityValidationFailureException<long>(typeof(TEntity).Name, entity.ID, this);
            }
        }

        /// <summary>
        /// Validates and throws <see cref="ZDatabase.Exceptions.ValidationFailureException"/> when the entity has errors.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the identifier.</typeparam>
        /// <param name="entityID">The identifier.</param>
        public void ValidateEntityErrors<TEntity, TKey>(TKey entityID)
            where TEntity : class
            where TKey : struct
        {
            if (HasErrors)
            {
                throw new EntityValidationFailureException<TKey>(typeof(TEntity).Name, entityID, this);
            }
        }

        /// <summary>
        /// Validates and throws <see cref="ZDatabase.Exceptions.ValidationFailureException"/> when has errors.
        /// </summary>
        /// <exception cref="ZDatabase.Exceptions.ValidationFailureException"></exception>
        public void ValidateErrors()
        {
            if (HasErrors)
            {
                throw new ValidationFailureException(this);
            }
        }
        #endregion

        #region Private methods
        #endregion
    }
}