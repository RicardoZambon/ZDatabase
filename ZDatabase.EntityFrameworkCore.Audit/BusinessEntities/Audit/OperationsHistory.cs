using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;

namespace ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit
{
    /// <summary>
    /// Abstract class for database history operations.
    /// </summary>
    /// <typeparam name="TServiceHistory">The type of the service history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.OperationsHistoryBase" />
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
    /// Entity configuration for <see cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.OperationsHistory{TServiceHistory, TUsers, TUsersKey}" />.
    /// </summary>
    /// <typeparam name="TOperationsHistoryEntity">The type of the operations history entity.</typeparam>
    /// <typeparam name="TServiceHistory">The type of the service history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.OperationHistoryBaseConfiguration{TOperationsHistoryEntity}" />
    public abstract class OperationHistoryConfiguration<TOperationsHistoryEntity, TServiceHistory, TUsers, TUsersKey>
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
                .HasForeignKey(x => x.ServiceHistoryID);
        }
    }
}