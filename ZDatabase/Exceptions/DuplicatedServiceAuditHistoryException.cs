namespace ZDatabase.Exceptions
{
    /// <summary>
    /// Represents errors that occurs when the database context finds duplicated <see cref="ZDatabase.Entities.Audit.ServicesHistory{TUsers, TUsersKey}"/>.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DuplicatedServiceAuditHistoryException
        : Exception
    {
    }
}