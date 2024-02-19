namespace ZDatabase.Exceptions
{
    /// <summary>
    /// Exception when entities are not found.
    /// </summary>
    public class EntityNotFoundException<TEntity> : Exception
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotFoundException{TEntity}"/> class.
        /// </summary>
        /// <param name="entityID">The entity identifier.</param>
        public EntityNotFoundException(object entityID)
            : base($"Entity '{typeof(TEntity).Name} ({entityID})' not found.")
        {
        }
    }
}