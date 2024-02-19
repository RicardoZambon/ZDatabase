using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZDatabase.Entities.Audit
{
    /// <summary>
    /// Abstract class for database history operations.
    /// </summary>
    /// <typeparam name="TServicesHistory">The type of the service history.</typeparam>
    /// <typeparam name="TOperationsHistory">The type of the operations history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.OperationsHistoryBase" />
    public abstract class OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        : OperationsHistoryBase
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Gets or sets the service history.
        /// </summary>
        /// <value>
        /// The service history.
        /// </value>
        public virtual TServicesHistory? ServiceHistory { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.Entities.Audit.OperationsHistory{TServiceHistory, TOperationsHistoryEntity, TUsers, TUsersKey}" />.
    /// </summary>
    /// <typeparam name="TOperationsHistoryEntity">The type of the operations history entity.</typeparam>
    /// <typeparam name="TServicesHistory">The type of the service history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.OperationHistoryBaseConfiguration{TOperationsHistoryEntity}" />
    public abstract class OperationsHistoryConfiguration<TOperationsHistoryEntity, TServicesHistory, TUsers, TUsersKey>
        : OperationHistoryBaseConfiguration<TOperationsHistoryEntity>
        where TOperationsHistoryEntity : OperationsHistory<TServicesHistory, TOperationsHistoryEntity, TUsers, TUsersKey>
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistoryEntity, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TOperationsHistoryEntity> builder)
        {
            base.Configure(builder);

            // ServiceHistory
            builder.HasOne(x => x.ServiceHistory)
                .WithMany(x => x.Operations)
                .HasForeignKey(x => x.ServiceHistoryID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}