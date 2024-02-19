using ZDatabase.Entities.Audit;

namespace ZDatabase.Repositories.Audit.Interfaces
{
    /// <summary>
    /// Repository for <see cref="OperationsHistory{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>
    /// </summary>
    public interface IOperationsHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Adds the history operation asynchronous.
        /// </summary>
        /// <param name="operationHistory">The history operation.</param>
        Task AddOperationHistoryAsync(TOperationsHistory operationHistory);

        /// <summary>
        /// Lists the history operations asynchronous.
        /// </summary>
        /// <param name="serviceHistoryId">The service history identifier.</param>
        /// <returns>Query with all history operations.</returns>
        IQueryable<TOperationsHistory> ListOperations(long serviceHistoryId);
    }
}