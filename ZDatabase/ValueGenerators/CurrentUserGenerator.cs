using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using ZDatabase.Services.Interfaces;

namespace ZDatabase.ValueGenerators
{
    /// <summary>
    /// Generates the current user for properties when an entity is added to a context.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    /// <seealso cref="Microsoft.EntityFrameworkCore.ValueGeneration.ValueGenerator{TUserKey}" />
    public class CurrentUserGenerator<TUserKey>
        : ValueGenerator<TUserKey>
        where TUserKey : struct
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override TUserKey Next(EntityEntry entry)
        {
            try
            {
                return entry.Context.GetService<ICurrentUserProvider<TUserKey>>().CurrentUserID ?? default;
            }
            catch (InvalidOperationException)
            {
                return default;
            }
            catch
            {
                throw;
            }
        }
    }
}