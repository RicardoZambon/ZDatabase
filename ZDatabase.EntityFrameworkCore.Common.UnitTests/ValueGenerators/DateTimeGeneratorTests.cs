using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;
using ZDatabase.EntityFrameworkCore.Common.ValueGenerators;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.ValueGenerators
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

            DbContextFake dbContext = DbContextFakeFactory.Create();
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