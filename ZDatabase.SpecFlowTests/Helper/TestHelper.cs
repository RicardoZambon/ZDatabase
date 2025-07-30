using FluentAssertions;

namespace ZDatabase.SpecFlowTests.Helper
{
    internal static class TestHelper
    {
        internal static Type GetEntityType(string entityName)
        {
            Type? entityType = Type.GetType($"ZDatabase.SpecFlowTests.Fakes.Entities.{entityName}")
                ?? Type.GetType($"ZDatabase.SpecFlowTests.Fakes.Entities.Audit.{entityName}");
            entityType.Should().NotBeNull($"Entity type '{entityName}' should exist.");

            return entityType!;
        }
    }
}