﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ZDatabase.EntityFrameworkCore.Common.ValueGenerators
{
    /// <summary>
    /// Generates the year from <see cref="DateTime.Now"/> for properties when an entity is added to a context.
    /// </summary>
    public class DateTimeYearGenerator
        : ValueGenerator<int>
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override int Next(EntityEntry entry)
        {
            return DateTime.Now.Year;
        }
    }
}