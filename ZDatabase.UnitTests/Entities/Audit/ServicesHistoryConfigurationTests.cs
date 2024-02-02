using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.Entities.Audit
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Entities.Audit.ServicesHistoryConfiguration{TServiceHistoryEntity, TUsers, TUsersKey}"/>.
    /// </summary>
    public class ServicesHistoryConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if ChangedByID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void ChangedByID_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? changedByIDProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.ChangedByID));
            changedByIDProperty.Should().NotBeNull();
            changedByIDProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if ChangedByID have a foreign key configured.
        /// </summary>
        [Fact]
        public void ChangedByID_Pass_HasForeignKey()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? changedByIDProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.ChangedByID));
            changedByIDProperty.Should().NotBeNull();
            changedByIDProperty!.IsForeignKey().Should().BeTrue();

            IForeignKey? foreignKey = changedByIDProperty.GetContainingForeignKeys().FirstOrDefault();
            foreignKey.Should().NotBeNull();
            foreignKey!.PrincipalEntityType.ClrType.Should().Be(typeof(UsersEntityFake));
            foreignKey!.DependentToPrincipal!.PropertyInfo!.Name.Should().Be(nameof(ServicesHistoryEntityFake.ChangedBy));
            foreignKey!.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        /// <summary>
        /// Test the configuration to check if ChangedByID have a value generator configured.
        /// </summary>
        [Fact]
        public void ChangedByID_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? changedByIDProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.ChangedByID));
            changedByIDProperty.Should().NotBeNull();
            changedByIDProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);

            Func<IProperty, IEntityType, ValueGenerator>? factory = changedByIDProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(changedByIDProperty, null!).GetType().Should().BeSameAs(typeof(CurrentUserGenerator<long>));
        }
    }
}