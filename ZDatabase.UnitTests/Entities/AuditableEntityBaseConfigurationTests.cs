using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Linq.Expressions;
using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.Entities
{
    /// Unit tests for <see cref="ZDatabase.Entities.AuditableEntityBaseConfiguration{TAuditableEntity}"/>.
    public class AuditableEntityBaseConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if CreatedOn have after saved behavior configured.
        /// </summary>
        [Fact]
        public void CreatedOn_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? createdOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.CreatedOn));
            createdOnProperty.Should().NotBeNull();
            createdOnProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if CreatedOn have a value generator configured.
        /// </summary>
        [Fact]
        public void CreatedOn_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? createdOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.CreatedOn));
            createdOnProperty.Should().NotBeNull();
            createdOnProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);

            Func<IProperty, IEntityType, ValueGenerator>? factory = createdOnProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(createdOnProperty, null!).GetType().Should().BeSameAs(typeof(DateTimeUtcGenerator));
        }

        /// <summary>
        /// Test the configuration to check if LastChangedOn have after saved behavior configured.
        /// </summary>
        [Fact]
        public void LastChangedOn_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedOn));
            lastChangedOnProperty.Should().NotBeNull();
            lastChangedOnProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        /// <summary>
        /// Test the configuration to check if LastChangedOn have before saved behavior configured.
        /// </summary>
        [Fact]
        public void LastChangedOn_Pass_HasBeforeSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedOn));
            lastChangedOnProperty.Should().NotBeNull();
            lastChangedOnProperty!.GetBeforeSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        /// <summary>
        /// Test the configuration to check if LastChangedOn have a value generator configured.
        /// </summary>
        [Fact]
        public void LastChangedOn_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedOn));
            lastChangedOnProperty.Should().NotBeNull();
            lastChangedOnProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAddOrUpdate);

            Func<IProperty, IEntityType, ValueGenerator>? factory = lastChangedOnProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(lastChangedOnProperty, null!).GetType().Should().BeSameAs(typeof(DateTimeUtcGenerator));
        }
    }
}