using System.Data;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public class RoleStore : IRoleStore<IdentityRole>
    {
        private readonly string _connectionString;

        public RoleStore(IOptions<PointMasterConfig> config)
        {
            _connectionString = config.Value.ConnectionString;
        }

        private IDbConnection Connection => new NpgsqlConnection(_connectionString);

        public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO roles (id, name, normalized_name, concurrency_stamp)
            VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp)";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            const string sql = @"
            UPDATE roles
            SET name = @Name,
                normalized_name = @NormalizedName,
                concurrency_stamp = @ConcurrencyStamp
            WHERE id = @Id";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            const string sql = "DELETE FROM roles WHERE id = @Id";
            using IDbConnection db = Connection;
            await db.ExecuteAsync(sql, new { role.Id });
            return IdentityResult.Success;
        }

        public async Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT
                id AS Id,
                name AS Name,
                normalized_name AS NormalizedName,
                concurrency_stamp AS ConcurrencyStamp
            FROM roles
            WHERE id = @Id";
            using IDbConnection db = Connection;
            return await db.QuerySingleOrDefaultAsync<IdentityRole>(sql, new { Id = roleId });
        }

        public async Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT
                id AS Id,
                name AS Name,
                normalized_name AS NormalizedName,
                concurrency_stamp AS ConcurrencyStamp
            FROM roles
            WHERE normalized_name = @NormalizedName";
            using IDbConnection db = Connection;
            return await db.QuerySingleOrDefaultAsync<IdentityRole>(sql, new { NormalizedName = normalizedRoleName });
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Id);

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }

    public class UserStore : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>
    {
        private readonly string _ConnectionString;

        public UserStore(IOptions<PointMasterConfig> config)
        {
            _ConnectionString = config.Value.ConnectionString;
        }

        private NpgsqlConnection Connection => new NpgsqlConnection(_ConnectionString);

        public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            const string sql = @"
            INSERT INTO users (id, username, normalized_username, password_hash, security_stamp, concurrency_stamp)
            VALUES (@Id, @UserName, @NormalizedUserName, @PasswordHash, @SecurityStamp, @ConcurrencyStamp)";
            using NpgsqlConnection db = Connection;
            await db.ExecuteAsync(sql, user);
            return IdentityResult.Success;
        }

        public async Task<IdentityUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT
                id AS Id,
                username AS UserName,
                normalized_username AS NormalizedUserName,
                password_hash AS PasswordHash,
                security_stamp AS SecurityStamp,
                concurrency_stamp AS ConcurrencyStamp
            FROM users
            WHERE normalized_username = @NormalizedUserName";
            using NpgsqlConnection db = Connection;
            return await db.QueryFirstOrDefaultAsync<IdentityUser>(sql, new { NormalizedUserName = normalizedUserName.ToUpper() });
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
            => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var sql = "DELETE FROM users WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            db.Execute(sql, new { user.Id });
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            const string sql = @"
            UPDATE users
            SET username = @UserName,
                normalized_username = @NormalizedUserName,
                password_hash = @PasswordHash,
                security_stamp = @SecurityStamp,
                concurrency_stamp = @ConcurrencyStamp
            WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            db.Execute(sql, user);
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            const string sql = @"
            SELECT
                id AS Id,
                username AS UserName,
                normalized_username AS NormalizedUserName,
                password_hash AS PasswordHash,
                security_stamp AS SecurityStamp,
                concurrency_stamp AS ConcurrencyStamp
            FROM users
            WHERE id = @Id";
            using NpgsqlConnection db = Connection;
            return await db.QueryFirstOrDefaultAsync<IdentityUser>(sql, new { Id = userId });
        }

        public void Dispose() { }
    }
}