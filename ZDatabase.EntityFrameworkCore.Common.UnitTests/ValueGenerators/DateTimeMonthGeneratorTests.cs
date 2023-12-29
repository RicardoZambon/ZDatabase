using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
{
    public class DateTimeMonthGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeMonthGenerator dateTimeMonthGenerator = new();

            // Act

            // Assert
            dateTimeMonthGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            int? receivedMonth = null;

            DbContextFake dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeMonthGenerator dateTimeMonthGenerator = new();

            // Act
            Action act = () =>
            {
                receivedMonth = dateTimeMonthGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedMonth.Should().Be(DateTime.Now.Month);
        }
    }
}