﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
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

            IDbContext dbContext = DbContextFakeFactory.Create();
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