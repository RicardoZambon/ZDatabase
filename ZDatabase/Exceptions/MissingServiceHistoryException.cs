namespace ZDatabase.Exceptions
{
    /// <summary>
    /// Represents errors that occurs when the database context can not find any <see cref="ZDatabase.Entities.Audit.ServicesHistory{TUsers, TUsersKey}"/>.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class MissingServiceHistoryException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingServiceHistoryException"/> class.
        /// </summary>
        public MissingServiceHistoryException()
            : base($"Missing services history when saving changes to auditable entities from database context.")
        {
        }
    }
}