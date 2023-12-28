using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Base
{
    /// <summary>
    /// Abstract class for base auditable database entities.
    /// </summary>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.Entity" />
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
        public DateTime LastChangesOn { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Base.AuditableEntityBase" />.
    /// </summary>
    /// <typeparam name="TAuditableEntity">The type of the auditable entity.</typeparam>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.EntityConfiguration{TAuditableEntity}" />
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

            // LastChangesOn
            builder.Property(x => x.LastChangesOn)
                .ValueGeneratedOnAddOrUpdate()
                .HasValueGenerator<DateTimeUtcGenerator>();

            builder.Property(x => x.LastChangesOn)
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);

            builder.Property(x => x.LastChangesOn)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
        }
    }
}