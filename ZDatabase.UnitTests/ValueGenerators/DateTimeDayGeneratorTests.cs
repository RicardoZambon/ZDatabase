using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
{
    public class DateTimeDayGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeDayGenerator dateTimeDayGenerator = new();

            // Act

            // Assert
            dateTimeDayGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            int? receivedDay = null;

            IDbContext dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeDayGenerator dateTimeDayGenerator = new();

            // Act
            Action act = () =>
            {
                receivedDay = dateTimeDayGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedDay.Should().Be(DateTime.Now.Day);
        }
    }
}