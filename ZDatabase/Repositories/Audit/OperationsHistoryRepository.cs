using ZDatabase.Entities.Audit;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit.Interfaces;

namespace ZDatabase.Repositories.Audit
{
    /// <inheritdoc />
    public class OperationsHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey> : IOperationsHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        #region Variables
        private readonly IDbContext dbContext;
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationsHistoryRepository{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="IDbContext"/> instance.</param>
        public OperationsHistoryRepository(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task AddOperationHistoryAsync(TOperationsHistory operationHistory)
        {
            await dbContext.AddAsync(operationHistory);
        }

        /// <inheritdoc />
        public IQueryable<TOperationsHistory> ListOperations<TEntity>(long serviceHistoryID)
            => from oh in dbContext.Set<TOperationsHistory>()
               where oh.ServiceHistoryID == serviceHistoryID
               select oh;
        #endregion

        #region Private methods
        #endregion
    }
}