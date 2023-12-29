using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
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

            DbContextFake dbContext = DbContextFakeFactory.Create();
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