using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.Entities.Audit
{
    /// Unit tests for <see cref="ZDatabase.Entities.Audit.ServicesHistoryBaseConfiguration{TServiceHistoryEntity}"/>.
    public class ServicesHistoryBaseConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if ChangedOn have after saved behavior configured.
        /// </summary>
        [Fact]
        public void ChangedOn_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? changedOnProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.ChangedOn));
            changedOnProperty.Should().NotBeNull();
            changedOnProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if ChangedOn have a value generator configured.
        /// </summary>
        [Fact]
        public void ChangedOn_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? changedOnProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.ChangedOn));
            changedOnProperty.Should().NotBeNull();
            changedOnProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);

            Func<IProperty, IEntityType, ValueGenerator>? factory = changedOnProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(changedOnProperty, null!).GetType().Should().BeSameAs(typeof(DateTimeUtcGenerator));
        }

        /// <summary>
        /// Test the configuration to check if Name have after saved behavior configured.
        /// </summary>
        [Fact]
        public void Name_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? nameProperty = dbContext.Model.FindEntityType(typeof(ServicesHistoryEntityFake))?.FindProperty(nameof(ServicesHistoryEntityFake.Name));
            nameProperty.Should().NotBeNull();
            nameProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }
    }
}