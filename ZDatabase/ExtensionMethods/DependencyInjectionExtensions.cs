using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using ZDatabase.Entities.Audit;
using ZDatabase.Repositories.Audit;
using ZDatabase.Repositories.Audit.Interfaces;
using ZDatabase.Services.Interfaces;

namespace ZDatabase.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds the audit handler service to the service collection.
        /// </summary>
        /// <typeparam name="TAuditHandler">The type of the audit handler.</typeparam>
        /// <param name="services">The services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAuditHandler<TAuditHandler>(this IServiceCollection services)
            where TAuditHandler : class, IAuditHandler
            => services
                .AddScoped<IAuditHandler, TAuditHandler>();

        /// <summary>
        /// Adds the audit repositories to the service collection.
        /// </summary>
        /// <typeparam name="TServicesHistory">The type of the services history.</typeparam>
        /// <typeparam name="TOperationsHistory">The type of the operations history.</typeparam>
        /// <typeparam name="TUsers">The type of the users.</typeparam>
        /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
        /// <param name="services">The services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAuditRepositories<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>(this IServiceCollection services)
            where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
            where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
            where TUsers : class
            where TUsersKey : struct
            => services
                .AddScoped<IOperationsHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>, OperationsHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>>()
                .AddScoped<IServicesHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>, ServicesHistoryRepository<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>>();

        /// <summary>
        /// Adds the current user provider to the service collection.
        /// </summary>
        /// <typeparam name="TCurrentUserProvider">The type of the current user provider.</typeparam>
        /// <typeparam name="TUserKey">The type of the user key.</typeparam>
        /// <param name="services">The services.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddCurrentUserProvider<TCurrentUserProvider, TUserKey>(this IServiceCollection services)
            where TCurrentUserProvider : class, ICurrentUserProvider<TUserKey>
            where TUserKey : struct
            => services
                .AddScoped<ICurrentUserProvider<TUserKey>, TCurrentUserProvider>();
    }
}