using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.ValueGenerators;

namespace ZDatabase.Entities
{
    /// <summary>
    /// Abstract class for auditable database entities.
    /// </summary>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="AuditableEntityBase" />
    public class AuditableEntity<TUsers, TUsersKey>
        : AuditableEntityBase
        where TUsers : class
        where TUsersKey : struct
    {
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public virtual TUsers? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the created by identifier.
        /// </summary>
        /// <value>
        /// The created by identifier.
        /// </value>
        public TUsersKey CreatedByID { get; set; }

        /// <summary>
        /// Gets or sets the last changed by.
        /// </summary>
        /// <value>
        /// The last changed by.
        /// </value>
        public virtual TUsers? LastChangedBy { get; set; }

        /// <summary>
        /// Gets or sets the last changed by identifier.
        /// </summary>
        /// <value>
        /// The last changed by identifier.
        /// </value>
        public TUsersKey LastChangedByID { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="AuditableEntity{TUsers, TUsersKey}"/>.
    /// </summary>
    /// <typeparam name="TAuditableEntity">The type of the auditable entity.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="AuditableEntityBaseConfiguration{TAuditableEntity}" />
    public class AuditableEntityConfiguration<TAuditableEntity, TUsers, TUsersKey>
        : AuditableEntityBaseConfiguration<TAuditableEntity>
        where TAuditableEntity : AuditableEntity<TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TAuditableEntity> builder)
        {
            base.Configure(builder);

            // CreatedBy
            builder.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByID)
                .OnDelete(DeleteBehavior.NoAction);

            // CreatedByID
            builder.Property(x => x.CreatedByID)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<CurrentUserGenerator<TUsersKey>>()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // LastChangedBy
            builder.HasOne(x => x.LastChangedBy)
                .WithMany()
                .HasForeignKey(x => x.LastChangedByID)
                .OnDelete(DeleteBehavior.NoAction);

            // LastChangedByID
            builder.Property(x => x.LastChangedByID)
                .ValueGeneratedOnAddOrUpdate()
                .HasValueGenerator<CurrentUserGenerator<TUsersKey>>();

            builder.Property(x => x.LastChangedByID)
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);

            builder.Property(x => x.LastChangedByID)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }
    }
}