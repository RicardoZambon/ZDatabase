using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.BusinessEntities
{
    public class EntityConfigurationTests
    {
        [Fact]
        public void Pass_HasDefaultValueForIsDeleted()
        {
            // Arrange
            IProperty? isDeletedProperty;

            // Act
            DbContextFake dbContext = DbContextFakeFactory.Create();

            // Assert
            isDeletedProperty = dbContext.Model.FindEntityType(typeof(EntityFake))?.FindProperty(nameof(Entity.IsDeleted));

            isDeletedProperty.Should().NotBeNull();

            object? defaultValue = isDeletedProperty!.GetDefaultValue();
            defaultValue.Should().NotBeNull();
            defaultValue.Should().Be(false);
        }

        [Fact]
        public void Pass_HasKeyForID()
        {
            // Arrange
            IProperty? idProperty;

            // Act
            DbContextFake dbContext = DbContextFakeFactory.Create();

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
            DbContextFake dbContext = DbContextFakeFactory.Create();

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