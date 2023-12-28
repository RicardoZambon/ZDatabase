using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit
{
    /// <summary>
    /// Abstract class for database history services.
    /// </summary>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.BaseEntity" />
    public abstract class ServicesHistoryBase
        : BaseEntity
    {
        /// <summary>
        /// Gets or sets the changed on.
        /// </summary>
        /// <value>
        /// The changed on.
        /// </value>
        [Column(TypeName = "DATETIME")]
        public DateTime ChangedOn { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [StringLength(200)]
        public string? Name { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.ServicesHistoryBase" />.
    /// </summary>
    /// <typeparam name="TServiceHistoryEntity">The type of the service history entity.</typeparam>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.BaseEntityConfiguration{TServiceHistoryEntity}" />
    public class ServicesHistoryBaseConfiguration<TServiceHistoryEntity>
        : BaseEntityConfiguration<TServiceHistoryEntity>
        where TServiceHistoryEntity : ServicesHistoryBase
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TServiceHistoryEntity> builder)
        {
            base.Configure(builder);

            // ChangedOn
            builder.Property(x => x.ChangedOn)
                .ValueGeneratedOnAdd()
                .HasValueGenerator<DateTimeUtcGenerator>();

            builder.Property(x => x.ChangedOn)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Name
            builder.Property(x => x.Name).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}