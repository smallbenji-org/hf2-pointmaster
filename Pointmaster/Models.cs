namespace Pointmaster
{
    public static class TenantRoles
    {
        public const string SuperUser = "SuperUser";
        public const string Administrator = "Administrator";
        public const string PostUser = "PostUser";
    }

    public class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class TenantMember
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string TenantId { get; set; }
        public string RoleName { get; set; }
    }

    public class Patrulje
    {
        public Patrulje() {}

        public Patrulje(string name, string tenantId)
        {
            this.Name = name;
            this.TenantId = tenantId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }

    public class Point
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int Turnout { get; set; }
        public string TenantId { get; set; }
        public Patrulje Patrulje { get; set; }
        public Post Post { get; set; }
    }

    public class Post
    {
        public Post() {}
        public Post(string name, string tenantId)
        {
            this.Name = name;
            this.TenantId = tenantId;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
    }

    public class PointMasterConfig
    {
        public string ConnectionString { get; set; }
    }
}