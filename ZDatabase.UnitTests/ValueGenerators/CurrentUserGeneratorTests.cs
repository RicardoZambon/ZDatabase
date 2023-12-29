using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute.ExceptionExtensions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.ValueGenerators;

namespace ZDatabase.UnitTests.ValueGenerators
{
    public class CurrentUserGeneratorTests
    {
        [Fact]
        public void GeneratesTemporaryValues_Pass_ReturnFalse()
        {
            // Arrange
            CurrentUserGenerator<long> currentUserGenerator = new();

            // Act

            // Assert
            currentUserGenerator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Fact]
        public void Next_Fail_ReturnDefaultWhenServiceIsNotFound()
        {
            // Arrange
            long? receivedID = null;

            IDbContext dbContext = DbContextFakeFactory.Create();
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            CurrentUserGenerator<long> currentUserGenerator = new();

            // Act
            Action act = () =>
            {
                receivedID = currentUserGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedID.Should().Be(default);
        }

        [Fact]
        public void Next_Fail_ThrowsException()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            currentUserProvider.CurrentUserID.Throws(new Exception("Test"));

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserProvider);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            CurrentUserGenerator<long> currentUserGenerator = new();

            // Act
            Action act = () =>
            {
                currentUserGenerator.Next(entry);
            };

            // Assert
            act.Should().Throw<Exception>().WithMessage("Test");

        }

        [Fact]
        public void Next_Pass_ReturnCurrentUserID()
        {
            // Arrange
            long expectedID = new Random().Next(1, 999_999);
            long? receivedID = null;

            ICurrentUserProvider<long> currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            currentUserProvider.CurrentUserID.Returns(expectedID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserProvider);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            CurrentUserGenerator<long> currentUserGenerator = new();

            // Act
            Action act = () =>
            {
                receivedID = currentUserGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedID.Should().Be(expectedID);
        }

        [Fact]
        public void Next_Pass_ReturnDefaultWhenCurrentUserIDIsNull()
        {
            // Arrange
            long? receivedID = null;

            ICurrentUserProvider<long> currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            currentUserProvider.CurrentUserID.Returns((long?)null);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserProvider);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);
            EntityFake entity = new();
            EntityEntry<EntityFake> entry = dbContext.Add(entity);

            CurrentUserGenerator<long> currentUserGenerator = new();

            // Act
            Action act = () =>
            {
                receivedID = currentUserGenerator.Next(entry);
            };

            // Assert
            act.Should().NotThrow();

            receivedID.Should().Be(default);
        }
    }
}