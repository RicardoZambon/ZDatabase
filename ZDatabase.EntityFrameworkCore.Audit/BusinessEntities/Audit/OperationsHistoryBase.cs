using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;

namespace ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit
{
    /// <summary>
    /// Abstract class for database base history operations.
    /// </summary>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.BaseEntity" />
    public abstract class OperationsHistoryBase
        : BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier.
        /// </summary>
        /// <value>
        /// The entity identifier.
        /// </value>
        public long? EntityID { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        /// <value>
        /// The name of the entity.
        /// </value>
        [Column(TypeName = "VARCHAR(100)")]
        public string? EntityName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the new values.
        /// </summary>
        /// <value>
        /// The new values.
        /// </value>
        public string? NewValues { get; set; }

        /// <summary>
        /// Gets or sets the old values.
        /// </summary>
        /// <value>
        /// The old values.
        /// </value>
        public string? OldValues { get; set; }

        /// <summary>
        /// Gets or sets the type of the operation.
        /// </summary>
        /// <value>
        /// The type of the operation.
        /// </value>
        [StringLength(50)]
        public string? OperationType { get; set; }

        /// <summary>
        /// Gets or sets the service history identifier.
        /// </summary>
        /// <value>
        /// The service history identifier.
        /// </value>
        public long ServiceHistoryID { get; set; }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        [Column(TypeName = "VARCHAR(100)")]
        public string? TableName { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit.OperationsHistoryBase" />.
    /// </summary>
    /// <typeparam name="TOperationsHistoryEntity">The type of the operations history entity.</typeparam>
    /// <seealso cref="ZDatabase.EntityFrameworkCore.Common.BusinessEntities.BaseEntityConfiguration&lt;TOperationsHistoryEntity&gt;" />
    public abstract class OperationHistoryBaseConfiguration<TOperationsHistoryEntity>
        : BaseEntityConfiguration<TOperationsHistoryEntity>
        where TOperationsHistoryEntity : OperationsHistoryBase
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TOperationsHistoryEntity> builder)
        {
            base.Configure(builder);

            // EntityID
            builder.Property(x => x.EntityID)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // EntityName
            builder.Property(x => x.EntityName)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // NewValues
            builder.Property(x => x.NewValues)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // OldValues
            builder.Property(x => x.OldValues)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // OperationType
            builder.Property(x => x.OperationType)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // ServiceHistoryID
            builder.Property(x => x.ServiceHistoryID)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // TableName
            builder.Property(x => x.TableName)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}