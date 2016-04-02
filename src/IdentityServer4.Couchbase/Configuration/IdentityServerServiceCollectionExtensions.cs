using System;
using Identity.Couchbase;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Validation;
using IdentityServer4.Couchbase;
using IdentityServer4.Couchbase.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerServiceCollectionExtensions
    {
        public static IServiceCollection AddCouchbaseTransientStores(this IServiceCollection services)
        {
            services.TryAddSingleton<IAuthorizationCodeStore, CouchbaseAuthorizationCodeStore>();
            services.TryAddSingleton<IRefreshTokenStore, CouchbaseRefreshTokenStore>();
            services.TryAddSingleton<ITokenHandleStore, CouchbaseTokenHandleStore>();
            services.TryAddSingleton<IConsentStore, CouchbaseConsentStore>();
            return services;
        }

        public static IIdentityServerBuilder AddCouchbaseScopes(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IScopeStore, CouchbaseScopeStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddCouchbaseUsers<TUser>(this IIdentityServerBuilder builder) where TUser : class, IIdentityUser, new()
        {
            builder.Services.AddTransient<IProfileService, AspNetIdentityProfileService<TUser>>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, CouchbaseResourceOwnerPasswordValidator<TUser>>();

            return builder;
        }

        public static IIdentityServerBuilder AddCouchbaseClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddTransient<IClientStore, CouchbaseClientStore>();
            builder.Services.AddTransient<ICorsPolicyService, CouchbaseCorsPolicyService>();

            return builder;
        }     
    }
}
