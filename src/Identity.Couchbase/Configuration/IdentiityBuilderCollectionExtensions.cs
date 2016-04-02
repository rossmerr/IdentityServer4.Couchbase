using System;
using Identity.Couchbase;
using Microsoft.AspNet.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentiityBuilderCollectionExtensions
    {
        public static IdentityBuilder AddCouchBaseStores<TUser, TRole>(this IdentityBuilder builder)
            where TUser : class, IIdentityUser, new()
            where TRole : class , IIdentityRole
        {
            builder.Services.AddSingleton<IUserStore<TUser>, UserStore<TUser, TRole>>();
            builder.Services.AddSingleton<IRoleStore<TRole>, RoleStore<TRole>>();
            return builder;
        }
    }
}
