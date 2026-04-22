using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public interface ITenantRepository
    {
        Task<List<Tenant>> GetTenantsForUser(string userId);
        Task<List<Tenant>> GetAllTenants();
        Task<Tenant> GetById(string tenantId);
        Task<Tenant> CreateTenant(string name);
        Task AddMember(string userId, string tenantId, string roleName);
        Task UpdateMemberRole(string userId, string tenantId, string roleName);
        Task<List<TenantMember>> GetMembers(string tenantId);
        Task<string> GetUserRole(string userId, string tenantId);
        Task<bool> IsSuperUser(string userId);
        Task SetSuperUser(string userId, bool isSuperUser);
    }

    public class TenantRepository : ITenantRepository
    {
        private readonly IOptions<PointMasterConfig> options;

        public TenantRepository(IOptions<PointMasterConfig> options)
        {
            this.options = options;
        }

        private IDbConnection Db => new NpgsqlConnection(options.Value.ConnectionString);

        public async Task<List<Tenant>> GetTenantsForUser(string userId)
        {
            const string sql = @"
                SELECT t.id, t.name
                FROM tenants t
                INNER JOIN user_tenants ut ON ut.tenant_id = t.id
                WHERE ut.user_id = @userId
                ORDER BY t.name
            ";

            using var conn = Db;
            return (await conn.QueryAsync<Tenant>(sql, new { userId })).ToList();
        }

        public async Task<List<Tenant>> GetAllTenants()
        {
            const string sql = @"
                SELECT id, name
                FROM tenants
                ORDER BY name
            ";
            using var conn = Db;
            return (await conn.QueryAsync<Tenant>(sql)).ToList();
        }

        public async Task<Tenant> GetById(string tenantId)
        {
            const string sql = @"
                SELECT id, name
                FROM tenants
                WHERE id = @tenantId
            ";
            using var conn = Db;
            return await conn.QueryFirstOrDefaultAsync<Tenant>(sql, new { tenantId });
        }

        public async Task<Tenant> CreateTenant(string name)
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = name.Trim()
            };

            const string sql = @"
                INSERT INTO tenants (id, name)
                VALUES (@Id, @Name)
            ";

            using var conn = Db;
            await conn.ExecuteAsync(sql, tenant);
            return tenant;
        }

        public async Task AddMember(string userId, string tenantId, string roleName)
        {
            const string sql = @"
                INSERT INTO user_tenants (user_id, tenant_id, role_name)
                VALUES (@userId, @tenantId, @roleName)
                ON CONFLICT (user_id, tenant_id)
                DO UPDATE SET role_name = EXCLUDED.role_name
            ";

            using var conn = Db;
            await conn.ExecuteAsync(sql, new { userId, tenantId, roleName });
        }

        public async Task UpdateMemberRole(string userId, string tenantId, string roleName)
        {
            const string sql = @"
                UPDATE user_tenants
                SET role_name = @roleName
                WHERE user_id = @userId AND tenant_id = @tenantId
            ";

            using var conn = Db;
            await conn.ExecuteAsync(sql, new { userId, tenantId, roleName });
        }

        public async Task<List<TenantMember>> GetMembers(string tenantId)
        {
            const string sql = @"
                SELECT ut.user_id, u.username, ut.tenant_id, ut.role_name
                FROM user_tenants ut
                INNER JOIN users u ON u.id = ut.user_id
                WHERE ut.tenant_id = @tenantId
                ORDER BY u.username
            ";

            using var conn = Db;
            return (await conn.QueryAsync<TenantMember>(sql, new { tenantId })).ToList();
        }

        public async Task<string> GetUserRole(string userId, string tenantId)
        {
            const string sql = @"
                SELECT role_name
                FROM user_tenants
                WHERE user_id = @userId AND tenant_id = @tenantId
            ";

            using var conn = Db;
            return await conn.QueryFirstOrDefaultAsync<string>(sql, new { userId, tenantId });
        }

        public async Task<bool> IsSuperUser(string userId)
        {
            const string sql = @"
                SELECT is_super_user
                FROM users
                WHERE id = @userId
            ";

            using var conn = Db;
            return await conn.QueryFirstOrDefaultAsync<bool>(sql, new { userId });
        }

        public async Task SetSuperUser(string userId, bool isSuperUser)
        {
            const string sql = @"
                UPDATE users
                SET is_super_user = @isSuperUser
                WHERE id = @userId
            ";

            using var conn = Db;
            await conn.ExecuteAsync(sql, new { userId, isSuperUser });
        }
    }
}
