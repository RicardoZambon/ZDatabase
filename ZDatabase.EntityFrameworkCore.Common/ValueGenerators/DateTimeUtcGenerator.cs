// Ignore Spelling: Utc

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ZDatabase.EntityFrameworkCore.Common.ValueGenerators
{
    /// <summary>
    /// Generates the date and time from <see cref="DateTime.UtcNow"/> for properties when an entity is added to a context.
    /// </summary>
    public class DateTimeUtcGenerator
        : ValueGenerator<DateTime>
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override DateTime Next(EntityEntry entry)
        {
            return DateTime.UtcNow;
        }
    }
}