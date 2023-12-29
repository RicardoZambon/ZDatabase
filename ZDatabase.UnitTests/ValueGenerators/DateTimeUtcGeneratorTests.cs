// Ignore Spelling: Utc

using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
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

            IDbContext dbContext = DbContextFakeFactory.Create();
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