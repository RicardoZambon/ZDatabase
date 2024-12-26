using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ZDatabase.Interfaces
{
    /// <summary>
    /// Interface for database injection.
    /// </summary>
    public interface IDbContext
    {
        /// <summary>
        /// Gets the change tracker.
        /// </summary>
        /// <value>
        /// The change tracker.
        /// </value>
        ChangeTracker ChangeTracker { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        DatabaseFacade Database { get; }

        /// <summary>
        ///     The metadata about the shape of entities, the relationships between them, and how they map to the database.
        ///     May not include all the information necessary to initialize the database.
        /// </summary>
        /// <remarks>
        ///     See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information.
        /// </remarks>
        IModel Model { get; }


        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        EntityEntry<TEntity> Add<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Adds the asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class;

        /// <summary>
        /// Clears the audit service history.
        /// </summary>
        void ClearAuditServiceHistory();

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="configureEntity">The configure entity.</param>
        /// <param name="constructorArguments">The constructor arguments.</param>
        /// <returns></returns>
        TEntity CreateProxy<TEntity>(Action<TEntity>? configureEntity, params object[] constructorArguments)
            where TEntity : class;

        /// <summary>
        /// Entries the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Finds the asynchronous.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="keyValues">The key values.</param>
        /// <returns></returns>
        ValueTask<TEntity?> FindAsync<TEntity>(params object?[]? keyValues)
            where TEntity : class;

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Saves the changes asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        EntityEntry<TEntity> Update<TEntity>(TEntity entity)
            where TEntity : class;
    }
}