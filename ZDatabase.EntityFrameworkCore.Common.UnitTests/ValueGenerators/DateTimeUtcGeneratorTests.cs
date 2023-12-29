using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
{
    public class DateTimeUtcGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeUtcGenerator dateTimeUtcGenerator = new();

            // Act

            // Assert
            dateTimeUtcGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            DateTime initialDateTime = DateTime.UtcNow;
            DateTime? receivedDateTime = null;

            DbContextFake dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeUtcGenerator dateTimeUtcGenerator = new();

            // Act
            Action act = () =>
            {
                receivedDateTime = dateTimeUtcGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedDateTime.Should().BeAfter(initialDateTime);
            receivedDateTime.Should().BeBefore(DateTime.UtcNow);
        }
    }
}