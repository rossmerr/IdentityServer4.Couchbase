using System;

namespace Identity.Couchbase
{
    public static class IdentityExtensions
    {

        static string ConvertEmailToId(string email)
        {
            return $"IdentityUser:{email}";
        }

        public static string ConvertUserToId(this IUser user) 
        {
            return ConvertEmailToId(user.Email);
        }

        public static string ConvertRoleToId(this IRole user) 
        {
            return $"IdentityRole:{user.RoleId}";
        }
    }
}
