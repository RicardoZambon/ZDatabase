using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Proxies.Internal;
using Microsoft.Extensions.DependencyInjection;
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

#pragma warning disable EF1001 // Internal EF Core API usage.
            ProxiesConventionSetPlugin proxiesConventionSubstitute = Substitute.For<ProxiesConventionSetPlugin>(null, null, null, null);
            proxiesConventionSubstitute.ModifyConventions(Arg.Any<ConventionSet>()).Returns(arg => arg.ArgAt<ConventionSet>(0));

            IProxyFactory proxyFactorySubstitute = Substitute.For<IProxyFactory>();
            proxyFactorySubstitute.Create(Arg.Any<DbContext>(), Arg.Any<Type>(), Arg.Any<object[]>())
                .Returns(arg => Activator.CreateInstance(arg.ArgAt<Type>(1), arg.ArgAt<object[]>(2)));

            ServiceProvider serviceProvider = serviceCollection
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton<IConventionSetPlugin>(proxiesConventionSubstitute)
                .AddSingleton(proxyFactorySubstitute)
                .BuildServiceProvider();
#pragma warning restore EF1001 // Internal EF Core API usage.

            return new ServiceCollection()
                .AddDbContext<IDbContext, DbContextFake>(options => options.UseInternalServiceProvider(serviceProvider))
                .AddScoped<IAuditHandler, AuditHandlerFake>()
                .BuildServiceProvider()
                .GetRequiredService<IDbContext>();
        }
    }
}