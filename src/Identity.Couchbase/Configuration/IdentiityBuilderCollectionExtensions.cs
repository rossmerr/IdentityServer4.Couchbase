using System;
using Identity.Couchbase;
using Identity.Couchbase.Stores;
using Microsoft.AspNet.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentiityBuilderCollectionExtensions
    {
        public static IdentityBuilder AddCouchBaseStores<TUser, TRole>(this IdentityBuilder builder)
            where TUser : IUser, new()
            where TRole : IRole
        {
            builder.Services.AddSingleton<IUserStore<TUser>, UserStore<TUser, TRole>>();
            builder.Services.AddSingleton<IRoleStore<TRole>, RoleStore<TRole>>();
            return builder;
        }
    }
}
