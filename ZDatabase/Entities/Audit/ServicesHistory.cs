using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.ValueGenerators;

namespace ZDatabase.Entities.Audit
{
    /// <summary>
    /// Abstract class for database history services.
    /// </summary>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.ServicesHistoryBase" />
    public abstract class ServicesHistory<TUsers, TUsersKey>
        : ServicesHistoryBase
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Gets or sets the changed by.
        /// </summary>
        /// <value>
        /// The changed by.
        /// </value>
        public virtual TUsers? ChangedBy { get; set; }

        /// <summary>
        /// Gets or sets the changed by identifier.
        /// </summary>
        /// <value>
        /// The changed by identifier.
        /// </value>
        public TUsersKey ChangedByID { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.Entities.Audit.ServicesHistory{TUsers, TUsersKey}" />.
    /// </summary>
    /// <typeparam name="TServiceHistoryEntity">The type of the service history entity.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Entities.Audit.ServicesHistoryBaseConfiguration{TServiceHistoryEntity}" />
    public class ServicesHistoryConfiguration<TServiceHistoryEntity, TUsers, TUsersKey>
        : ServicesHistoryBaseConfiguration<TServiceHistoryEntity>
        where TServiceHistoryEntity : ServicesHistory<TUsers, TUsersKey>
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
        }
    }
}