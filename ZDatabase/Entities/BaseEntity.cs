using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZDatabase.Entities
{
    /// <summary>
    /// Abstract class for base database entities.
    /// </summary>
    public abstract class BaseEntity
    {
    }

    /// <summary>
    /// Entity configuration for <see cref="BaseEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}" />
    public abstract class BaseEntityConfiguration<TEntity>
        : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity
    {
        /// <inheritdoc />
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
        }
    }
}