using Couchbase.Linq.Filters;

namespace Identity.Couchbase
{
    [DocumentTypeFilter("session")]
    public class Session
    {
        public Session()
        {
            Type = "session";
        }

        public Session(string data) : this()
        {
            Data = data;
        }

        public string Data { get; set; }
        public string Type { get; set; }
    }
}
