using System;

namespace Identity.Couchbase
{
    public static class IdentityUserExtensions
    {

        public static string ConvertEmailToId(string email)
        {
            return $"IdentityUser:{email}";
        }

        public static string ConvertUserToId(this IIdentityUser user) 
        {
            return ConvertEmailToId(user.Email);
        }

        public static string ConvertRoleToId(this IIdentityRole user) 
        {
            return $"IdentityRole:{user.RoleId}";
        }
    }
}
