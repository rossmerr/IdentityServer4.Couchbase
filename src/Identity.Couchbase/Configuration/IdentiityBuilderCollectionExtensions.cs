using System;
using Identity.Couchbase;
using Identity.Couchbase.Stores;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentiityBuilderCollectionExtensions
    {
        public static IdentityBuilder AddCouchbaseStores<TUser, TRole>(this IdentityBuilder builder)
            where TUser : IUser, new()
            where TRole : IRole
        {
            builder.Services.AddSingleton<IUserStore<TUser>, UserStore<TUser, TRole>>();
            builder.Services.AddSingleton<IRoleStore<TRole>, RoleStore<TRole>>();
            return builder;
        }

        public static IdentityBuilder AddCouchbaseSessionStores(this IdentityBuilder builder)
        {
            builder.Services.TryAddSingleton<ITicketStore, SessionStore>();
            return builder;
        }
    }
}
