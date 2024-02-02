using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Entities
{
    /// Unit tests for <see cref="ZDatabase.Entities.EntityConfiguration{TEntity}"/>.
    public class EntityConfigurationTests
    {
        /// <summary>
        /// Test the configuration to check if ID have the IsKey configured.
        /// </summary>
        [Fact]
        public void ID_Pass_HasKey()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? idProperty = dbContext.Model.FindEntityType(typeof(EntityFake))?.FindProperty(nameof(EntityFake.ID));
            idProperty.Should().NotBeNull();
            idProperty!.IsKey().Should().BeTrue();
            idProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        /// <summary>
        /// Test the configuration to check if IsDeleted have the default value configured.
        /// </summary>
        [Fact]
        public void IsDeleted_Pass_HasDefaultValue()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            IProperty? isDeletedProperty = dbContext.Model.FindEntityType(typeof(EntityFake))?.FindProperty(nameof(Entity.IsDeleted));
            isDeletedProperty.Should().NotBeNull();

            object? defaultValue = isDeletedProperty!.GetDefaultValue();
            defaultValue.Should().NotBeNull();
            defaultValue.Should().Be(false);
        }

        /// <summary>
        /// Test the configuration to check if have the query filter configured.
        /// </summary>
        [Fact]
        public void Pass_HasQueryFilterConfigured()
        {
            // Arrange

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            LambdaExpression? queryFilter = dbContext.Model.FindEntityType(typeof(EntityFake))?.GetQueryFilter();
            queryFilter.Should().NotBeNull();

            queryFilter!.Body.Should().BeAssignableTo<UnaryExpression>();
            UnaryExpression body = (UnaryExpression)queryFilter.Body;

            body.Operand.Should().BeAssignableTo<MemberExpression>();
            MemberExpression member = (MemberExpression)body.Operand;

            member.Member.Name.Should().Be(nameof(Entity.IsDeleted));
        }
    }
}