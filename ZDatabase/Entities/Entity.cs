using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using ZDatabase.Interfaces;

namespace ZDatabase.Entities
{
    /// <summary>
    /// Abstract class for database entities.
    /// </summary>
    /// <seealso cref="ZDatabase.Entities.BaseEntity" />
    /// <seealso cref="ZDatabase.Interfaces.ISoftDelete" />
    public abstract class Entity : BaseEntity, ISoftDelete
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the row version.
        /// </summary>
        /// <value>
        /// The row version.
        /// </value>
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }

    /// <summary>
    /// Entity configuration for <see cref="Entity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}" />
    public abstract class EntityConfiguration<TEntity>
        : BaseEntityConfiguration<TEntity>
        where TEntity : Entity
    {
        /// <inheritdoc />
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.HasQueryFilter(x => !x.IsDeleted);

            // IsDeleted
            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}