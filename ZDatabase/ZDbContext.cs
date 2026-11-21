using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;

namespace ZDatabase
{
    /// <inheritdoc />    
    public abstract class ZDbContext<TDbContext> : DbContext, IDbContext
        where TDbContext : DbContext, IDbContext
    {
        #region Variables
        #endregion

        #region Properties
        /// <summary>
        /// Gets the audit handler.
        /// </summary>
        /// <value>
        /// The audit handler.
        /// </value>
        protected IAuditHandler AuditHandler { get => this.GetService<IAuditHandler>(); }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ZDatabase.ZDbContext{TDbContext}"/> class.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.EntityFrameworkCore.DbContextOptions{TDbContext}"/> instance.</param>
        public ZDbContext(DbContextOptions<TDbContext> options)
            : base(options)
        {

        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void ClearAuditServiceHistory()
        {
            AuditHandler?.ClearServiceHistory();
        }

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="configureEntity">The configure entity.</param>
        /// <param name="constructorArguments">The constructor arguments.</param>
        /// <returns>The proxy instance.</returns>
        public TEntity CreateProxy<TEntity>(Action<TEntity>? configureEntity, params object[] constructorArguments) where TEntity : class
        {
            return ProxiesExtensions.CreateProxy(this, configureEntity, constructorArguments);
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TDbContext).Assembly);
        }

        /// <inheritdoc />
        /// <remarks>
        /// On a relational provider, the data and its audit rows are saved in one transaction, so a
        /// failed audit save rolls the data back. This is skipped when the caller already has a
        /// transaction (the save joins it), when automatic transactions are turned off, and when the
        /// execution strategy retries on failure (wrap the whole unit of work in the strategy instead).
        /// </remarks>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (!ShouldWrapSaveInTransaction())
            {
                return SaveChangesWithAudit(acceptAllChangesOnSuccess);
            }

            using IDbContextTransaction transaction = Database.BeginTransaction();

            int result = SaveChangesWithAudit(acceptAllChangesOnSuccess);

            transaction.Commit();

            return result;
        }

        /// <inheritdoc />
        /// <remarks>
        /// On a relational provider, the data and its audit rows are saved in one transaction, so a
        /// failed audit save rolls the data back. This is skipped when the caller already has a
        /// transaction (the save joins it), when automatic transactions are turned off, and when the
        /// execution strategy retries on failure (wrap the whole unit of work in the strategy instead).
        /// </remarks>
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            if (!ShouldWrapSaveInTransaction())
            {
                return await SaveChangesWithAuditAsync(acceptAllChangesOnSuccess, cancellationToken);
            }

            await using IDbContextTransaction transaction = await Database.BeginTransactionAsync(cancellationToken);

            int result = await SaveChangesWithAuditAsync(acceptAllChangesOnSuccess, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        #endregion

        #region Private methods
        private bool ShouldWrapSaveInTransaction()
        {
            // The audit rows are written by a nested save; it joins the transaction opened here.
            return Database.IsRelational()
                && Database.CurrentTransaction is null
                && Database.GetEnlistedTransaction() is null
                && Transaction.Current is null
                && Database.AutoTransactionBehavior != AutoTransactionBehavior.Never
                && !Database.CreateExecutionStrategy().RetriesOnFailure;
        }

        private int SaveChangesWithAudit(bool acceptAllChangesOnSuccess)
        {
            IAuditHandler auditHandler = AuditHandler;

            if (auditHandler is not null)
            {
                auditHandler.RefreshAuditedEntries(ChangeTracker);
                auditHandler.AddOperationEntitiesBeforeSaving();
            }

            int result = base.SaveChanges(acceptAllChangesOnSuccess);

            auditHandler?.AddOperationEntitiesAfterSaved();

            return result;
        }

        private async Task<int> SaveChangesWithAuditAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
        {
            IAuditHandler auditHandler = AuditHandler;

            if (auditHandler is not null)
            {
                auditHandler.RefreshAuditedEntries(ChangeTracker);
                await auditHandler.AddOperationEntitiesBeforeSavingAsync();
            }

            int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (auditHandler is not null)
            {
                await auditHandler.AddOperationEntitiesAfterSavedAsync();
            }

            return result;
        }
        #endregion
    }
}