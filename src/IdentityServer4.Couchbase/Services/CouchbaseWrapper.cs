namespace IdentityServer4.Couchbase.Services
{
    class CouchbaseWrapper<TEntity>
    {
        public CouchbaseWrapper(string id, TEntity model)
        {
            Id = id;
            Model = model;
        }

        public string Id { get; set; }
        public TEntity Model { get; set; }
    }
}