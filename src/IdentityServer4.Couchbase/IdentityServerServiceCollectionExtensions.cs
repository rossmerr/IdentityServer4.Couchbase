using Identity.Couchbase;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Validation;
using IdentityServer4.Couchbase.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityServer4.Couchbase
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
            builder.Services.AddSingleton<IScopeStore, CouchbaseScopeStore>();
            return builder;
        }

        public static IIdentityServerBuilder AddCouchbaseUsers<TUser>(this IIdentityServerBuilder builder) where TUser : IUser
        {
            builder.Services.AddSingleton<IProfileService, AspNetIdentityProfileService<TUser>>();
            builder.Services.AddTransient<IResourceOwnerPasswordValidator, CouchbaseResourceOwnerPasswordValidator<TUser>>();
            return builder;
        }

        public static IIdentityServerBuilder AddCouchbaseClients(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<ICouchbaseClientStore, CouchbaseClientStore>();
            builder.Services.AddSingleton<IClientStore>(p => p.GetService<ICouchbaseClientStore>());

            builder.Services.AddSingleton<ICouchbaseScopeStore, CouchbaseScopeStore>();
            builder.Services.AddSingleton<IScopeStore>(p => p.GetService<ICouchbaseScopeStore>());

            builder.Services.AddSingleton<ICorsPolicyService, CouchbaseCorsPolicyService>();
            return builder;
        }     
    }
}
