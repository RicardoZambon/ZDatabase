using ZDatabase.Entities;
using ZDatabase.Entities.Audit;

namespace ZDatabase.Repositories.Audit.Interfaces
{
    /// <summary>
    /// Repository for <see cref="ServicesHistory{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>
    /// </summary>
    public interface IServicesHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Adds the history service asynchronous.
        /// </summary>
        /// <param name="serviceHistory">The history service.</param>
        Task AddServiceHistoryAsync(TServicesHistory serviceHistory);

        /// <summary>
        /// Lists the history services asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entityID">The entity identifier.</param>
        /// <returns>Query with all history services.</returns>
        Task<IQueryable<TServicesHistory>> ListServicesAsync<TEntity>(long entityID)
            where TEntity : AuditableEntity<TUsers, TUsersKey>;
    }
}