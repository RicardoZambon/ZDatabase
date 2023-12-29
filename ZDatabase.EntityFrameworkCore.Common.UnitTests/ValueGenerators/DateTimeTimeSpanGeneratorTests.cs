using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
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

            DbContextFake dbContext = DbContextFakeFactory.Create();
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