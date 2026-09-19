using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Entities.Audit
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Entities.Audit.ServicesHistory{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>.
    /// </summary>
    public class ServicesHistoryTests
    {
        /// <summary>
        /// Test the Operations should default to null.
        /// </summary>
        [Fact]
        public void Operations_Pass_DefaultsToNull()
        {
            // Arrange

            // Act
            ServicesHistoryEntityFake serviceHistory = new();

            // Assert
            serviceHistory.Operations.Should().BeNull();
        }

        /// <summary>
        /// Test the Operations should round-trip the assigned collection.
        /// </summary>
        [Fact]
        public void Operations_Pass_RoundTripsCollection()
        {
            // Arrange
            ServicesHistoryEntityFake serviceHistory = new();
            List<OperationsHistoryEntityFake> operations = [new(), new()];

            // Act
            serviceHistory.Operations = operations;

            // Assert
            serviceHistory.Operations.Should().BeSameAs(operations);
            serviceHistory.Operations.Should().HaveCount(2);
        }

        /// <summary>
        /// Test the Operations should accept an empty collection.
        /// </summary>
        [Fact]
        public void Operations_Pass_AcceptsEmptyCollection()
        {
            // Arrange
            ServicesHistoryEntityFake serviceHistory = new();

            // Act
            serviceHistory.Operations = [];

            // Assert
            serviceHistory.Operations.Should().BeEmpty();
        }

        /// <summary>
        /// Test the ChangedByID should round-trip the assigned value.
        /// </summary>
        [Theory]
        [InlineData(0L)]
        [InlineData(42L)]
        [InlineData(long.MaxValue)]
        public void ChangedByID_Pass_RoundTripsValue(long changedByID)
        {
            // Arrange
            ServicesHistoryEntityFake serviceHistory = new();

            // Act
            serviceHistory.ChangedByID = changedByID;

            // Assert
            serviceHistory.ChangedByID.Should().Be(changedByID);
        }
    }
}
