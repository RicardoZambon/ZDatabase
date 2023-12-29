using Microsoft.Extensions.DependencyInjection;
using ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.Factories
{
    internal static class DbContextFakeFactory
    {
        internal static DbContextFake Create(ServiceCollection? serviceCollection = null)
        {
            if (serviceCollection == null)
            {
                serviceCollection = new ServiceCollection();
            }

            ServiceProvider serviceProvider = serviceCollection
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            return new ServiceCollection()
                .AddDbContext<DbContextFake>(options => options.UseInternalServiceProvider(serviceProvider))
                .BuildServiceProvider()
                .GetRequiredService<DbContextFake>();
        }
    }
}