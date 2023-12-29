using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.BusinessEntities
{
    public class EntityTests
    {
        [Fact]
        public void ID_Pass()
        {
            // Arrange
            Entity entity = Substitute.For<Entity>();

            // Act
            entity.ID = new Random().Next(1, 999_999);

            // Assert
            entity.ID.Should().BeGreaterThan(0);
        }

        [Fact]
        public void IsDeleted_Pass()
        {
            // Arrange
            Entity entity = Substitute.For<Entity>();

            // Act
            entity.IsDeleted = true;

            // Assert
            entity.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void RowVersion_Pass()
        {
            // Arrange
            Entity entity = Substitute.For<Entity>();

            // Act
            entity.RowVersion = Array.Empty<byte>();

            // Assert
            entity.RowVersion.Should().NotBeNull();
        }
    }
}
