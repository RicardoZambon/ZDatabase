using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Entities.Audit
{
    /// Unit tests for <see cref="ZDatabase.Entities.Audit.OperationsHistoryConfiguration{TOperationsHistoryEntity, TServiceHistory, TUsers, TUsersKey}"/>.
    public class OperationsHistoryConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if ServiceHistoryID have a foreign key configured.
        /// </summary>
        [Fact]
        public void ServiceHistoryID_Pass_HasForeignKey()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? serviceHistoryIDProperty = dbContext.Model.FindEntityType(typeof(OperationsHistoryEntityFake))?.FindProperty(nameof(OperationsHistoryEntityFake.ServiceHistoryID));
            serviceHistoryIDProperty.Should().NotBeNull();
            serviceHistoryIDProperty!.IsForeignKey().Should().BeTrue();

            IForeignKey? foreignKey = serviceHistoryIDProperty.GetContainingForeignKeys().FirstOrDefault();
            foreignKey.Should().NotBeNull();
            foreignKey!.PrincipalEntityType.ClrType.Should().Be(typeof(ServicesHistoryEntityFake));
            foreignKey!.DependentToPrincipal!.PropertyInfo!.Name.Should().Be(nameof(OperationsHistoryEntityFake.ServiceHistory));
            foreignKey!.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }
    }
}