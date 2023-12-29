using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
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

            IDbContext dbContext = DbContextFakeFactory.Create();
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