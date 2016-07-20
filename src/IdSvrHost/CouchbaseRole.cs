using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace IdSvrHost
{
    public class CouchbaseRole : IRole
    {
        public CouchbaseRole()
        {
            Claims = new List<Claim>();
        }
    }
}
