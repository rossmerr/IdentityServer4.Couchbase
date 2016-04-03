namespace IdentityServer4.Couchbase.Wrappers
{
    public class CouchbaseWrapper<TEntity>
    {
        public CouchbaseWrapper(string id, TEntity model)
        {
            Id = id;
            Model = model;
        }

        public string Id { get; set; }
        public TEntity Model { get; set; }
        public string Discriminator => typeof(TEntity).Name.ToLower();
    }
}