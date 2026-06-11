using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ZDatabase.Entities;
using ZDatabase.Entities.Audit;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;

namespace ZDatabase.Initializers
{
    /// <summary>
    /// Abstract class for database initialization.
    /// </summary>
    /// <typeparam name="TServicesHistory">The type of the service history.</typeparam>
    public abstract class BaseInitializer<TServicesHistory>
        where TServicesHistory : ServicesHistoryBase, new()
    {
        #region Variables

        /// <summary>
        /// The database context.
        /// </summary>
        protected readonly IDbContext dbContext;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseInitializer{TServicesHistory}"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="IDbContext"/> instance.</param>
        public BaseInitializer(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the audit handler.
        /// </summary>
        /// <value>
        /// The audit handler.
        /// </value>
        protected IAuditHandler? AuditHandler
        {
            get => (dbContext as DbContext)?.GetService<IAuditHandler>();
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void Initialize();

        #endregion

        #region Protected methods

        /// <summary>
        /// Begins the transaction and registers a new service history entry.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <returns>The <see cref="IDbContextTransaction"/> instance.</returns>
        protected IDbContextTransaction BeginTransaction(string methodName)
        {
            IDbContextTransaction transaction = dbContext.Database.BeginTransaction();
            dbContext.Set<TServicesHistory>().Add(new TServicesHistory { Name = $"{GetType().Name}\\{methodName}" });
            return transaction;
        }

        /// <summary>
        /// Saves the entities into the context, assigning new identifiers to unmatched entities and skipping entities already present.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="propertyCheck">The property used to check whether an entity already exists.</param>
        protected void SaveContext<TEntity, TValue>(IEnumerable<TEntity> entities, string methodName, Func<TEntity, TValue> propertyCheck)
            where TEntity : Entity
        {
            long maxID = 1L;
            if (dbContext.Set<TEntity>().Any())
            {
                maxID = dbContext.Set<TEntity>().IgnoreQueryFilters().Max(x => x.ID) + 1;
            }

            if (entities.Max(x => x.ID) is long entitiesMaxID && entitiesMaxID > maxID)
            {
                maxID = entitiesMaxID + 1;
            }

            foreach (TEntity entity in entities)
            {
                TValue? propertyValue = new TEntity[] { entity }.Select(propertyCheck).FirstOrDefault();

                if (!dbContext.Set<TEntity>().Select(propertyCheck).Any(x => EqualityComparer<TValue>.Default.Equals(x, propertyValue)) && entity.ID <= 0)
                {
                    entity.ID = maxID;
                    maxID++;
                }
                else if (dbContext.Set<TEntity>().Select(propertyCheck).Any(x => EqualityComparer<TValue>.Default.Equals(x, propertyValue)) && entity.ID > 0)
                {
                    entity.ID = 0;
                }
            }

            SaveContext(entities.Where(x => x.ID > 0), methodName, entriesState: EntityState.Added, forceIdentityInsert: true);
        }

        /// <summary>
        /// Saves the entities into the context.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="entriesState">State of the entries.</param>
        /// <param name="forceIdentityInsert">if set to <c>true</c> [force identity insert].</param>
        protected void SaveContext<TEntity>(IEnumerable<TEntity> entities, string methodName, EntityState entriesState = EntityState.Added, bool forceIdentityInsert = false)
            where TEntity : class
        {
            using IDbContextTransaction transaction = BeginTransaction(methodName);
            try
            {
                foreach (TEntity entity in entities)
                {
                    switch (entriesState)
                    {
                        case EntityState.Added:
                            dbContext.Set<TEntity>().Add(entity);
                            break;
                        case EntityState.Modified:
                            dbContext.Set<TEntity>().Update(entity);
                            break;
                        case EntityState.Deleted:
                            dbContext.Set<TEntity>().Remove(entity);
                            break;
                    }
                }

                if (forceIdentityInsert && entriesState == EntityState.Added)
                {
                    SetIdentityInsert<TEntity>(true);
                }

                dbContext.SaveChanges();

                if (forceIdentityInsert && entriesState == EntityState.Added)
                {
                    SetIdentityInsert<TEntity>(false);
                }

                transaction.Commit();

                AuditHandler?.ClearServiceHistory();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            dbContext.ChangeTracker.Clear();
        }

        /// <summary>
        /// Enables or disables explicit identity value inserts for <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        /// <remarks>
        /// The default implementation does nothing. Providers that require explicit identity-insert
        /// toggling (e.g. SQL Server <c>SET IDENTITY_INSERT</c>) should override this method.
        /// </remarks>
        protected virtual void SetIdentityInsert<TEntity>(bool enable)
            where TEntity : class
        {
        }

        #endregion
    }
}
