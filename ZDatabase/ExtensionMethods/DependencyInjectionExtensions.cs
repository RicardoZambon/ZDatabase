using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
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
            => services.AddScoped<IAuditHandler, TAuditHandler>();

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
            => services.AddScoped<ICurrentUserProvider<TUserKey>, TCurrentUserProvider>();
    }
}