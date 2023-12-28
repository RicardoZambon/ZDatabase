namespace ZDatabase.EntityFrameworkCore.Audit.Exceptions
{
    /// <summary>
    /// Represents errors that occurs when the database context finds duplicated <see cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.ServicesHistory{TUsers, TUsersKey}"/>.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DuplicatedServiceAuditHistoryException
        : Exception
    {
    }
}