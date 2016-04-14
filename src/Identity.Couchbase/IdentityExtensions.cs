using System;
using Microsoft.AspNet.Identity;

namespace Identity.Couchbase
{
    public static class IdentityExtensions
    {
        public static string ConvertSessionKeyToId(this string sessionKey, ILookupNormalizer lookupNormalizer)
        {
            return lookupNormalizer.Normalize($"IdentitySession:{sessionKey}");
        }

        public static string ConvertEmailToId(string email, ILookupNormalizer lookupNormalizer)
        {
            return lookupNormalizer.Normalize($"IdentityUser:{email}");
        }

        public static string ConvertUserToId(this IUser user, ILookupNormalizer lookupNormalizer) 
        {
            return ConvertEmailToId(user.Email, lookupNormalizer);
        }

        public static string ConvertRoleToId(this IRole user, ILookupNormalizer lookupNormalizer)
        {
            return lookupNormalizer.Normalize($"IdentityRole:{user.RoleId}".ToLower());
        }
    }
}
