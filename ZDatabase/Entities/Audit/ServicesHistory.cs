using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using ZDatabase.ValueGenerators;

namespace ZDatabase.Entities.Audit
{
    /// <summary>
    /// Abstract class for database history services.
    /// </summary>
    /// <typeparam name="TServicesHistory">The type of the service history.</typeparam>
    /// <typeparam name="TOperationsHistory">The type of the operations history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.ServicesHistoryBase" />
    public abstract class ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        : ServicesHistoryBase
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Gets or sets the changed by.
        /// </summary>
        /// <value>
        /// The changed by.
        /// </value>
        [ExcludeFromCodeCoverage]
        public virtual TUsers? ChangedBy { get; set; }

        /// <summary>
        /// Gets or sets the changed by identifier.
        /// </summary>
        /// <value>
        /// The changed by identifier.
        /// </value>
        public TUsersKey ChangedByID { get; set; }

        /// <summary>
        /// Gets or sets the operations.
        /// </summary>
        /// <value>
        /// The operations.
        /// </value>
        public virtual ICollection<TOperationsHistory>? Operations { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.Entities.Audit.ServicesHistory{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}" />.
    /// </summary>
    /// <typeparam name="TServiceHistoryEntity">The type of the service history entity.</typeparam>
    /// <typeparam name="TOperationsHistory">The type of the operations history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.ServicesHistoryBaseConfiguration{TServiceHistoryEntity}" />
    public class ServicesHistoryConfiguration<TServiceHistoryEntity, TOperationsHistory, TUsers, TUsersKey> : ServicesHistoryBaseConfiguration<TServiceHistoryEntity>
        where TServiceHistoryEntity : ServicesHistory<TServiceHistoryEntity, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServiceHistoryEntity, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TServiceHistoryEntity> builder)
        {
            base.Configure(builder);

            // ChangedBy
            builder.HasOne(x => x.ChangedBy)
                .WithMany()
                .HasForeignKey(x => x.ChangedByID)
                .OnDelete(DeleteBehavior.NoAction);

            // ChangedByID
            builder.Property(x => x.ChangedByID)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<CurrentUserGenerator<TUsersKey>>();

            builder.Property(x => x.ChangedByID)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Operations
            builder.HasMany(x => x.Operations)
                .WithOne(x => x.ServiceHistory)
                .HasForeignKey(x => x.ServiceHistoryID);
        }
    }
}