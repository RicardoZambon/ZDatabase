using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;

namespace ZDatabase.UnitTests
{
    public class ZDbContextTests
    {
        [Fact]
        public void CreateProxy_Pass_CreateProxyEntry()
        {
            // Arrange
            EntityFake? entityFake = null;

            IDbContext dbContext = DbContextFakeFactory.Create();

            // Act
            Action act = () =>
            {
                entityFake = dbContext.CreateProxy<EntityFake>(x => { });
            };

            // Assert
            act.Should().NotThrow();

            entityFake.Should().NotBeNull();
        }

        [Fact]
        public void SaveChanges_Pass_ShouldHaveCalledAuditHandlerMethods()
        {
            // Arrange
            IAuditHandler auditHandlerSubstitute = Substitute.For<IAuditHandler>();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(auditHandlerSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            auditHandlerSubstitute.Received().RefreshAuditedEntries(Arg.Any<ChangeTracker>());
            auditHandlerSubstitute.Received().AddOperationEntitiesBeforeSaving();
            auditHandlerSubstitute.Received().AddOperationEntitiesAfterSaved();
        }

        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveCalledAuditHandlerMethods()
        {
            // Arrange
            IAuditHandler auditHandlerSubstitute = Substitute.For<IAuditHandler>();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(auditHandlerSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            auditHandlerSubstitute.Received().RefreshAuditedEntries(Arg.Any<ChangeTracker>());
            await auditHandlerSubstitute.Received().AddOperationEntitiesBeforeSavingAsync();
            await auditHandlerSubstitute.Received().AddOperationEntitiesAfterSavedAsync();
        }
    }
}