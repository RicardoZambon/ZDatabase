using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Linq.Expressions;
using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;

namespace ZDatabase.UnitTests.Entities
{
    public class AuditableEntityConfigurationTests
    {
        [Fact]
        public void CreatedOn_Pass_HasDefaultValue()
        {
            // Arrange
            IProperty? createdOnProperty;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            createdOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityBase.CreatedOn));

            createdOnProperty.Should().NotBeNull();

            createdOnProperty!.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        [Fact]
        public void CreatedOn_Pass_HasValueGenerator()
        {
            // Arrange
            IProperty? createdOnProperty;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            createdOnProperty = dbContext.Model.FindEntityType(typeof(AuditableEntityFake))?.FindProperty(nameof(AuditableEntityBase.CreatedOn));
            createdOnProperty.Should().NotBeNull();

            Func<IProperty, IEntityType, ValueGenerator>? factory = createdOnProperty!.GetValueGeneratorFactory();
            factory.Should().NotBeNull();

            // .Should().Be(ValueGenerated.OnAdd);
        }

        [Fact]
        public void Pass_HasKeyForID()
        {
            // Arrange
            IProperty? idProperty;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            idProperty = dbContext.Model.FindEntityType(typeof(EntityFake))?.FindProperty(nameof(Entity.ID));

            idProperty.Should().NotBeNull();
            idProperty!.IsKey().Should().BeTrue();
        }

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