using System;
using Couchbase.Linq.Filters;

namespace Identity.Couchbase
{
    [DocumentTypeFilter("user")]
    class UserWrapper<TUser> where TUser : class, IUser
    {
        public UserWrapper()
        {
            Type = "user";
        }

        public UserWrapper(TUser user) : this()
        {
            User = user;
        } 

        public TUser User { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
    }
}
