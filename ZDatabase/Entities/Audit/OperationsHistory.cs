using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZDatabase.Entities.Audit
{
    /// <summary>
    /// Abstract class for database history operations.
    /// </summary>
    /// <typeparam name="TServiceHistory">The type of the service history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.OperationsHistoryBase" />
    public abstract class OperationsHistory<TServiceHistory, TUsers, TUsersKey>
        : OperationsHistoryBase
        where TServiceHistory : ServicesHistory<TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Gets or sets the service history.
        /// </summary>
        /// <value>
        /// The service history.
        /// </value>
        public virtual TServiceHistory? ServiceHistory { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.Entities.Audit.OperationsHistory{TServiceHistory, TUsers, TUsersKey}" />.
    /// </summary>
    /// <typeparam name="TOperationsHistoryEntity">The type of the operations history entity.</typeparam>
    /// <typeparam name="TServiceHistory">The type of the service history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.OperationHistoryBaseConfiguration{TOperationsHistoryEntity}" />
    public abstract class OperationsHistoryConfiguration<TOperationsHistoryEntity, TServiceHistory, TUsers, TUsersKey>
        : OperationHistoryBaseConfiguration<TOperationsHistoryEntity>
        where TOperationsHistoryEntity : OperationsHistory<TServiceHistory, TUsers, TUsersKey>
        where TServiceHistory : ServicesHistory<TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TOperationsHistoryEntity> builder)
        {
            base.Configure(builder);

            // ServiceHistory
            builder.HasOne(x => x.ServiceHistory)
                .WithMany()
                .HasForeignKey(x => x.ServiceHistoryID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}