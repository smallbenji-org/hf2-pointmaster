using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public interface IPostRepository
    {
        Task AddPost(Post data);
        Task<Post> GetPostById(int Id, string tenantId);
        Task<List<Post>> GetAll(string tenantId);
        Task DeletePost(int Id, string tenantId);
    }

    public class DummyPostRepository : IPostRepository
    {
        private List<Post> _posts = [];
        private int idCount = 0;

        public async Task<Post> GetPostById(int Id, string tenantId)
        {
            return _posts.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task AddPost(Post data)
        {
            data.Id = idCount;
            idCount++;
            _posts.Add(data);
        }

        public async Task<List<Post>> GetAll(string tenantId)
        {
            return _posts.Where(x => x.TenantId == tenantId).ToList();
        }

        public async Task DeletePost(int Id, string tenantId)
        {
            var post = _posts.FirstOrDefault(x => x.Id == Id && x.TenantId == tenantId);
            _posts.Remove(post);
        }
    }

    public class PostRepository : IPostRepository
    {
        private readonly IOptions<PointMasterConfig> options;

        public PostRepository(IOptions<PointMasterConfig> options)
        {
            this.options = options;
        }

        private IDbConnection db => new NpgsqlConnection(options.Value.ConnectionString);

        public async Task AddPost(Post data)
        {
            const string sql = @"
            INSERT INTO poster (name, tenant_id)
            VALUES (@Name, @TenantId)
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, data);
        }

        public async Task DeletePost(int Id, string tenantId)
        {
            const string sql = @"
            DELETE FROM poster
            WHERE id = @id AND tenant_id = @tenantId
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, new { id = Id, tenantId });
        }

        public async Task<List<Post>> GetAll(string tenantId)
        {
            const string sql = @"
            SELECT
                *
            FROM poster
            WHERE tenant_id = @tenantId
            ";
            using var conn = db;
            return (await conn.QueryAsync<Post>(sql, new { tenantId })).ToList();
        }

        public async Task<Post> GetPostById(int Id, string tenantId)
        {
            const string sql = @"
            SELECT
                *
            FROM poster
            WHERE id = @id AND tenant_id = @tenantId
            ";
            using var conn = db;
            return await conn.QueryFirstOrDefaultAsync<Post>(sql, new { id = Id, tenantId });
        }
    }
}