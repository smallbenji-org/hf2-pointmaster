using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public interface IPostRepository
    {
        Task AddPost(Post data);
        Task<Post> GetPostById(int Id);
        Task<List<Post>> GetAll();
        Task DeletePost(int Id);
    }

    public class DummyPostRepository : IPostRepository
    {
        private List<Post> _posts = [];
        private int idCount = 0;

        public async Task<Post> GetPostById(int Id)
        {
            return _posts.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task AddPost(Post data)
        {
            data.Id = idCount;
            idCount++;
            _posts.Add(data);
        }

        public async Task<List<Post>> GetAll()
        {
            return _posts;
        }

        public async Task DeletePost(int Id)
        {
            var post = _posts.FirstOrDefault(x => x.Id == Id);
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
            INSERT INTO poster (name)
            VALUES (@Name)
            ";
            using var conn = db;
            await db.ExecuteAsync(sql, data);
        }

        public async Task DeletePost(int Id)
        {
            const string sql = @"
            DELETE FROM poster
            WHERE id = @id
            ";
            using var conn = db;
            await db.ExecuteAsync(sql, new { id = Id });
        }

        public async Task<List<Post>> GetAll()
        {
            const string sql = @"
            SELECT
                *
            FROM poster
            ";
            using var conn = db;
            return (await db.QueryAsync<Post>(sql)).ToList();
        }

        public async Task<Post> GetPostById(int Id)
        {
            const string sql = @"
            SELECT
                *
            FROM poster
            WHERE id = @id
            ";
            using var conn = db;
            return await db.QueryFirstAsync(sql, new { id = Id });
        }
    }
}