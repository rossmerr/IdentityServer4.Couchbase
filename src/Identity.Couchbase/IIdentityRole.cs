namespace Identity.Couchbase
{
    public interface IIdentityRole
    {
        string RoleId { get; set; }

        string NormalizedName { get; }
        string Name { get; }
    }
}