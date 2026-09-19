using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace ZDatabase.UnitTests.Helpers
{
    /// <summary>
    /// Test helpers for reading model metadata consistently across EF Core majors.
    /// </summary>
    internal static class EntityTypeTestExtensions
    {
        /// <summary>
        /// Gets the single query filter declared on an entity type.
        /// </summary>
        /// <param name="entityType">The entity type to read the filter from.</param>
        /// <returns>The query filter expression, or <see langword="null"/> when none is declared.</returns>
        /// <remarks>
        /// EF Core 10 introduced named query filters and obsoleted <c>GetQueryFilter()</c>
        /// in favour of <c>GetDeclaredQueryFilters()</c>.
        /// </remarks>
        internal static LambdaExpression? GetSingleQueryFilter(this IReadOnlyEntityType entityType)
#if NET10_0_OR_GREATER
            => entityType.GetDeclaredQueryFilters().SingleOrDefault()?.Expression;
#else
            => entityType.GetQueryFilter();
#endif
    }
}
