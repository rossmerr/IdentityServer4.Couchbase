using Identity.Couchbase.Stores;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Identity.Couchbase
{
    public static class IdentiityBuilderCollectionExtensions
    {
        public static IdentityBuilder AddCouchbaseStores<TUser, TRole>(this IdentityBuilder builder)
            where TUser : class, IUser
            where TRole : class, IRole
        {
            builder.Services.AddSingleton<UserStore<TUser, TRole>>();
            builder.Services.AddSingleton<IUserStore<TUser>>(p => p.GetService<UserStore<TUser, TRole>>());
            builder.Services.AddSingleton<ISubject<TUser>>(p => p.GetService<UserStore<TUser, TRole>>());
            builder.Services.AddSingleton<IRoleStore<TRole>, RoleStore<TRole>>();
            builder.Services.AddSingleton<ILookupNormalizer, LookupNormalizer>();

            return builder;
        }

        public static IdentityBuilder AddCouchbaseSessionStores(this IdentityBuilder builder)
        {
            builder.Services.TryAddSingleton<ITicketStore, SessionStore>();
            return builder;
        }
    }
}
