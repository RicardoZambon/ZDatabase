using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.UnitTests.Fakes.ServicesFake;

namespace ZDatabase.UnitTests.Factories
{
    internal static class DbContextFakeFactory
    {
        internal static IDbContext Create(ServiceCollection? serviceCollection = null)
        {
            serviceCollection ??= new ServiceCollection();

            ServiceProvider serviceProvider = serviceCollection
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            return new ServiceCollection()
                .AddDbContext<IDbContext, DbContextFake>(options => options.UseInternalServiceProvider(serviceProvider))
                .AddScoped<IAuditHandler, AuditHandlerFake>()
                .BuildServiceProvider()
                .GetRequiredService<IDbContext>();
        }
    }
}