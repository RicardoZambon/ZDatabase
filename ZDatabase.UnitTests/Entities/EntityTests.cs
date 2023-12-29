using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;

namespace ZDatabase.UnitTests.Entities
{
    public class EntityTests
    {
        [Fact]
        public async Task ID_Fail_AddingDuplicatedKeyAsync()
        {
            // Arrange
            long entityID = 1;

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new EntityFake { ID = entityID });
            await dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.AddAsync(new EntityFake { ID = entityID });
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().Where(x => x.Message.Contains("another instance with the same key value for {'ID'} is already being tracked"));
        }

        [Fact]
        public void ID_Pass_HasKey()
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
        public async Task ID_Pass_AddEntitiesAsync()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new EntityFake { ID = 1 });
            await dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.AddAsync(new EntityFake { ID = 2 });
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public void IsDeleted_Pass_HasDefaultValue()
        {
            // Arrange
            IProperty? isDeletedProperty;

            // Act
            IDbContext dbContext = DbContextFakeFactory.Create();

            // Assert
            isDeletedProperty = dbContext.Model.FindEntityType(typeof(EntityFake))?.FindProperty(nameof(Entity.IsDeleted));

            isDeletedProperty.Should().NotBeNull();

            object? defaultValue = isDeletedProperty!.GetDefaultValue();
            defaultValue.Should().NotBeNull();
            defaultValue.Should().Be(false);
        }

        [Fact]
        public async Task IsDeleted_Pass_DefaultValueIsFalse()
        {
            // Arrange
            long entityID = 1;

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new EntityFake { ID = entityID });
            await dbContext.SaveChangesAsync();

            // Act
            EntityFake? entity = await dbContext.FindAsync<EntityFake>(entityID);

            // Assert
            entity.Should().NotBeNull();
            entity!.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task IsDeleted_Pass_QueryShouldReturnOnlyNotDeletedAsync()
        {
            // Arrange
            IEnumerable<EntityFake> entities = new List<EntityFake>()
            {
                new() { },
                new() { },
                new() { },
                new() { IsDeleted = true },
                new() { IsDeleted = true },
            };

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.Set<EntityFake>().AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();

            // Act
            IEnumerable<EntityFake> foundEntities = dbContext.Set<EntityFake>();

            // Assert
            foundEntities.Should().NotBeNull();
            foundEntities.Should().HaveCount(entities.Count(x => !x.IsDeleted));
        }

        [Fact]
        public void RowVersion_Pass()
        {
            // Arrange
            Entity entity = new EntityFake
            {
                RowVersion = Array.Empty<byte>(),
            };

            // Act

            // Assert
            entity.RowVersion.Should().NotBeNull();
        }
    }
}
