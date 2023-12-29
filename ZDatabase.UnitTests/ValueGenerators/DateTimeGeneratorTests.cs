using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
{
    public class DateTimeGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeGenerator dateTimeGenerator = new();

            // Act

            // Assert
            dateTimeGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            DateTime initialDateTime = DateTime.Now;
            DateTime? receivedDateTime = null;

            IDbContext dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeGenerator dateTimeGenerator = new();

            // Act
            Action act = () =>
            {
                receivedDateTime = dateTimeGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedDateTime.Should().BeAfter(initialDateTime);
            receivedDateTime.Should().BeBefore(DateTime.Now);
        }
    }
}