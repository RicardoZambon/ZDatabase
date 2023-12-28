using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ZDatabase.EntityFrameworkCore.Common.ValueGenerators
{
    /// <summary>
    /// Generates the time span from <see cref="DateTime.Now"/> for properties when an entity is added to a context.
    /// </summary>
    public class DateTimeTimeSpanGenerator
        : ValueGenerator<TimeSpan>
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override TimeSpan Next(EntityEntry entry)
        {
            return DateTime.Now.TimeOfDay;
        }
    }
}