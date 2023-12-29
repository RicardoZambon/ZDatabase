using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
{
    public class DateTimeYearGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            DateTimeYearGenerator dateTimeYearGenerator = new();

            // Act

            // Assert
            dateTimeYearGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Pass_ReturnCurrentDay()
        {
            // Arrange
            int? receivedYear = null;

            DbContextFake dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            DateTimeYearGenerator dateTimeYearGenerator = new();

            // Act
            Action act = () =>
            {
                receivedYear = dateTimeYearGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedYear.Should().Be(DateTime.Now.Year);
        }
    }
}