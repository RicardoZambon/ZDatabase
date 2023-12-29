using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.ValueGenerators;

namespace ZDatabase.Entities
{
    /// <summary>
    /// Abstract class for base auditable database entities.
    /// </summary>
    /// <seealso cref="Entity" />
    public abstract class AuditableEntityBase
        : Entity
    {
        /// <summary>
        /// Gets or sets the created on.
        /// </summary>
        /// <value>
        /// The created on.
        /// </value>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the last changes on.
        /// </summary>
        /// <value>
        /// The last changes on.
        /// </value>
        public DateTime LastChangedOn { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="AuditableEntityBase" />.
    /// </summary>
    /// <typeparam name="TAuditableEntity">The type of the auditable entity.</typeparam>
    /// <seealso cref="EntityConfiguration{TAuditableEntity}" />
    public class AuditableEntityBaseConfiguration<TAuditableEntity>
        : EntityConfiguration<TAuditableEntity>
        where TAuditableEntity : AuditableEntityBase
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TAuditableEntity> builder)
        {
            base.Configure(builder);

            // CreatedOn
            builder.Property(x => x.CreatedOn)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<DateTimeUtcGenerator>()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // LastChangedOn
            builder.Property(x => x.LastChangedOn)
                .ValueGeneratedOnAddOrUpdate()
                .HasValueGenerator<DateTimeUtcGenerator>();

            builder.Property(x => x.LastChangedOn)
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);

            builder.Property(x => x.LastChangedOn)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }
    }
}