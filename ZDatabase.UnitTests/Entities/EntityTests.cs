using ZDatabase.Entities;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Entities
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Entities.Entity"/>.
    /// </summary>
    public class EntityTests
    {
        /// <summary>
        /// Test the ID should round-trip the assigned value.
        /// </summary>
        [Theory]
        [InlineData(0L)]
        [InlineData(1L)]
        [InlineData(long.MaxValue)]
        public void ID_Pass_RoundTripsValue(long id)
        {
            // Arrange
            EntityFake entity = new();

            // Act
            entity.ID = id;

            // Assert
            entity.ID.Should().Be(id);
        }

        /// <summary>
        /// Test the ID should default to zero.
        /// </summary>
        [Fact]
        public void ID_Pass_DefaultsToZero()
        {
            // Arrange

            // Act
            EntityFake entity = new();

            // Assert
            entity.ID.Should().Be(0L);
        }

        /// <summary>
        /// Test the IsDeleted should round-trip the assigned value.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsDeleted_Pass_RoundTripsValue(bool isDeleted)
        {
            // Arrange
            EntityFake entity = new();

            // Act
            entity.IsDeleted = isDeleted;

            // Assert
            entity.IsDeleted.Should().Be(isDeleted);
        }

        /// <summary>
        /// Test the IsDeleted should default to false.
        /// </summary>
        [Fact]
        public void IsDeleted_Pass_DefaultsToFalse()
        {
            // Arrange

            // Act
            EntityFake entity = new();

            // Assert
            entity.IsDeleted.Should().BeFalse();
        }

        /// <summary>
        /// Test the RowVersion should round-trip the assigned value.
        /// </summary>
        [Fact]
        public void RowVersion_Pass_RoundTripsValue()
        {
            // Arrange
            EntityFake entity = new();
            byte[] rowVersion = [1, 2, 3, 4];

            // Act
            entity.RowVersion = rowVersion;

            // Assert
            entity.RowVersion.Should().BeSameAs(rowVersion);
        }

        /// <summary>
        /// Test the RowVersion should default to null.
        /// </summary>
        [Fact]
        public void RowVersion_Pass_DefaultsToNull()
        {
            // Arrange

            // Act
            EntityFake entity = new();

            // Assert
            entity.RowVersion.Should().BeNull();
        }

        /// <summary>
        /// Test the RowVersion should accept null.
        /// </summary>
        [Fact]
        public void RowVersion_Pass_AcceptsNull()
        {
            // Arrange
            EntityFake entity = new() { RowVersion = [1] };

            // Act
            entity.RowVersion = null;

            // Assert
            entity.RowVersion.Should().BeNull();
        }

        /// <summary>
        /// Test the entity should implement the soft delete contract.
        /// </summary>
        [Fact]
        public void Entity_Pass_ImplementsSoftDelete()
        {
            // Arrange

            // Act
            EntityFake entity = new();

            // Assert
            entity.Should().BeAssignableTo<ISoftDelete>();
            entity.Should().BeAssignableTo<BaseEntity>();
        }
    }
}
