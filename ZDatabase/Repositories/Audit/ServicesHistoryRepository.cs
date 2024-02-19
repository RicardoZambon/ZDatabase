using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Entities;
using ZDatabase.Entities.Audit;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit.Interfaces;

namespace ZDatabase.Repositories.Audit
{
    /// <inheritdoc />
    public class ServicesHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey> : IServicesHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
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
        /// Initializes a new instance of the <see cref="ZDatabase.Repositories.Audit.ServicesHistoryRepository{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="IDbContext"/> instance.</param>
        public ServicesHistoryRepository(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task AddServiceHistoryAsync(TServicesHistory serviceHistory)
        {
            await dbContext.AddAsync(serviceHistory);
        }

        /// <inheritdoc />
        public async Task<IQueryable<TServicesHistory>> ListServicesAsync<TEntity>(long entityID)
            where TEntity : AuditableEntity<TUsers, TUsersKey>
        {
            if (await dbContext.FindAsync<TEntity>(entityID) is not TEntity entity)
            {
                throw new EntityNotFoundException<TEntity>(entityID);
            }

            EntityEntry<TEntity> entry = dbContext.Entry(entity);

            string tableName = entry.Metadata.GetTableName() ?? string.Empty;

            return from sh in dbContext.Set<TServicesHistory>()
                   where sh.Operations != null
                        && sh.Operations.Any(o =>
                            EF.Functions.Like(o.TableName ?? string.Empty, tableName)
                            && o.EntityID == entityID
                        )
                   orderby sh.ChangedOn descending
                   select sh;
        }
        #endregion

        #region Private methods
        #endregion
    }
}