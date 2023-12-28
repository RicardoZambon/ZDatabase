using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using ZDatabase.EntityFrameworkCore.Audit.Services.Interfaces;
using ZDatabase.EntityFrameworkCore.Common.Interfaces;

namespace ZDatabase.EntityFrameworkCore
{
    /// <inheritdoc />    
    public abstract class AppDbContext<TDbContext>
        : DbContext, IDbContext
        where TDbContext : DbContext, IDbContext
    {
        #region Variables
        #endregion

        #region Properties
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ZDatabase.EntityFrameworkCore.AppDbContext{TDbContext}"/> class.
        /// </summary>
        /// <param name="options">The <see cref="Microsoft.EntityFrameworkCore.DbContextOptions{TDbContext}"/> instance.</param>
        public AppDbContext(DbContextOptions<TDbContext> options)
            : base(options)
        {

        }
        #endregion

        #region Public methods
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

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext<>).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TDbContext).Assembly);
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            IAuditHandler auditHandler = this.GetService<IAuditHandler>();

            if (auditHandler is not null)
            {
                auditHandler.RefreshAuditedEntries(ChangeTracker);
                auditHandler.AddOperationEntitiesBeforeSaving();
            }

            int result = base.SaveChanges(acceptAllChangesOnSuccess);

            auditHandler?.AddOperationEntitiesAfterSaved();

            return result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            IAuditHandler auditHandler = this.GetService<IAuditHandler>();

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

        #region Private methods
        #endregion
    }
}