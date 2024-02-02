using Microsoft.EntityFrameworkCore.Metadata;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Entities.Audit
{
    /// Unit tests for <see cref="ZDatabase.Entities.Audit.OperationHistoryBaseConfiguration{TOperationsHistoryEntity}"/>.
    public class OperationsHistoryBaseConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if EntityID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void EntityID_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? entityIDProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.EntityID));
            entityIDProperty.Should().NotBeNull();
            entityIDProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if EntityName have after saved behavior configured.
        /// </summary>
        [Fact]
        public void EntityName_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? entityNameProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.EntityName));
            entityNameProperty.Should().NotBeNull();
            entityNameProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if NewValues have after saved behavior configured.
        /// </summary>
        [Fact]
        public void NewValues_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? newValuesProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.NewValues));
            newValuesProperty.Should().NotBeNull();
            newValuesProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if OldValues have after saved behavior configured.
        /// </summary>
        [Fact]
        public void OldValues_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? oldValuesProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.OldValues));
            oldValuesProperty.Should().NotBeNull();
            oldValuesProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if EntityID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void OperationType_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? operationTypeProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.OperationType));
            operationTypeProperty.Should().NotBeNull();
            operationTypeProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if ServiceHistoryID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void ServiceHistoryID_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? serviceHistoryIDProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.ServiceHistoryID));
            serviceHistoryIDProperty.Should().NotBeNull();
            serviceHistoryIDProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if TableName have after saved behavior configured.
        /// </summary>
        [Fact]
        public void TableName_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? tableNameProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.TableName));
            tableNameProperty.Should().NotBeNull();
            tableNameProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }
    }
}