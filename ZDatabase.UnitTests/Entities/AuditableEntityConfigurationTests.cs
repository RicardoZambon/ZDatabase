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
    /// Unit tests for <see cref="ZDatabase.Entities.AuditableEntityConfiguration{TAuditableEntity, TUsers, TUsersKey}"/>.
    public class AuditableEntityConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if CreatedByID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void CreatedByID_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? createdByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.CreatedByID));
            createdByIDProperty.Should().NotBeNull();
            createdByIDProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        /// <summary>
        /// Test the configuration to check if CreatedByID have a foreign key configured.
        /// </summary>
        [Fact]
        public void CreatedByID_Pass_HasForeignKey()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? createdByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.CreatedByID));
            createdByIDProperty.Should().NotBeNull();
            createdByIDProperty!.IsForeignKey().Should().BeTrue();
            
            IForeignKey? foreignKey = createdByIDProperty.GetContainingForeignKeys().FirstOrDefault();
            foreignKey.Should().NotBeNull();
            foreignKey!.PrincipalEntityType.ClrType.Should().Be(typeof(UsersEntityFake));
            foreignKey!.DependentToPrincipal!.PropertyInfo!.Name.Should().Be(nameof(AuditableEntityFake.CreatedBy));
            foreignKey!.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        /// <summary>
        /// Test the configuration to check if CreatedByID have a value generator configured.
        /// </summary>
        [Fact]
        public void CreatedByID_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? createdByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.CreatedByID));
            createdByIDProperty.Should().NotBeNull();
            createdByIDProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);

            Func<IProperty, IEntityType, ValueGenerator>? factory = createdByIDProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(createdByIDProperty, null!).GetType().Should().BeSameAs(typeof(CurrentUserGenerator<long>));
        }

        /// <summary>
        /// Test the configuration to check if LastChangedByID have after saved behavior configured.
        /// </summary>
        [Fact]
        public void LastChangedByID_Pass_HasAfterSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedByID));
            lastChangedByIDProperty.Should().NotBeNull();
            lastChangedByIDProperty!.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        /// <summary>
        /// Test the configuration to check if LastChangedByID have before saved behavior configured.
        /// </summary>
        [Fact]
        public void LastChangedByID_Pass_HasBeforeSavedBehavior()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedByID));
            lastChangedByIDProperty.Should().NotBeNull();
            lastChangedByIDProperty!.GetBeforeSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        /// <summary>
        /// Test the configuration to check if LastChangedByID have a foreign key configured.
        /// </summary>
        [Fact]
        public void LastChangedByID_Pass_HasForeignKey()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedByID));
            lastChangedByIDProperty.Should().NotBeNull();
            lastChangedByIDProperty!.IsForeignKey().Should().BeTrue();

            IForeignKey? foreignKey = lastChangedByIDProperty.GetContainingForeignKeys().FirstOrDefault();
            foreignKey.Should().NotBeNull();
            foreignKey!.PrincipalEntityType.ClrType.Should().Be(typeof(UsersEntityFake));
            foreignKey!.DependentToPrincipal!.PropertyInfo!.Name.Should().Be(nameof(AuditableEntityFake.LastChangedBy));
            foreignKey!.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        /// <summary>
        /// Test the configuration to check if LastChangedByID have a value generator configured.
        /// </summary>
        [Fact]
        public void LastChangedByID_Pass_HasValueGenerator()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? lastChangedByIDProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityFake.LastChangedByID));
            lastChangedByIDProperty.Should().NotBeNull();
            lastChangedByIDProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAddOrUpdate);

            Func<IProperty, IEntityType, ValueGenerator>? factory = lastChangedByIDProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory!.Invoke(lastChangedByIDProperty, null!).GetType().Should().BeSameAs(typeof(CurrentUserGenerator<long>));
        }


        /// <summary>
        /// Test the class to have configured a query filter.
        /// </summary>
        [Fact]
        public void Pass_HasQueryFilterConfigured()
        {
            // Arrange
            LambdaExpression? queryFilter;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            queryFilter = dbContext.Model.FindEntityType(typeof(EntityFake))?.GetQueryFilter();

            queryFilter.Should().NotBeNull();

            queryFilter!.Body.Should().BeAssignableTo<UnaryExpression>();
            UnaryExpression body = (UnaryExpression)queryFilter.Body;

            body.Operand.Should().BeAssignableTo<MemberExpression>();
            MemberExpression member = (MemberExpression)body.Operand;

            member.Member.Name.Should().Be(nameof(Entity.IsDeleted));
        }
    }
}