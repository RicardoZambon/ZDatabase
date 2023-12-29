using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ZDatabase.ValueGenerators
{
    /// <summary>
    /// Generates the current day from <see cref="DateTime.Now" /> for properties when an entity is added to a context.
    /// </summary>
    public class DateTimeDayGenerator
        : ValueGenerator<int>
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override int Next(EntityEntry entry)
        {
            return DateTime.Now.Day;
        }
    }
}