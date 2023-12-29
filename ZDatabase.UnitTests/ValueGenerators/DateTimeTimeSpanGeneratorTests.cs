using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
{
    public class DateTimeTimeSpanGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeTimeSpanGenerator dateTimeTimeSpanGenerator = new();

            // Act

            // Assert
            dateTimeTimeSpanGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            TimeSpan initialTimeSpan = DateTime.Now.TimeOfDay;
            TimeSpan? receivedTimeSpan = null;

            IDbContext dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeTimeSpanGenerator dateTimeTimeSpanGenerator = new();

            // Act
            Action act = () =>
            {
                receivedTimeSpan = dateTimeTimeSpanGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedTimeSpan.Should().BeGreaterThan(initialTimeSpan);
            receivedTimeSpan.Should().BeLessThan(DateTime.Now.TimeOfDay);
        }
    }
}