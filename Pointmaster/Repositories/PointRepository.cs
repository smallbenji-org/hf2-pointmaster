using System.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public interface IPointRepository
    {
        Task AddPoint(Point point);
        Task AddPointRange(List<Point> points);
        Task<Point> GetPointById(int Id, string tenantId);
        Task<List<Point>> GetPointByPatrulje(int PatruljeId, string tenantId);
        Task<Point> GetPointByPatruljeAndPost(int PatruljeId, int PostId, string tenantId);
        Task<Point> SavePoint(Point point);
        Task<List<Point>> GetAll(string tenantId);
        Task DeletePoint(int Id, string tenantId);
    }

    public class DummyPointRepository : IPointRepository
    {
        private List<Point> _points = [];
        private int idCount = 0;

        public async Task<Point> GetPointById(int Id, string tenantId)
        {
            return _points.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task<List<Point>> GetPointByPatrulje(int PatruljeId, string tenantId)
        {
            return _points.Where(x => x.Patrulje.Id.Equals(PatruljeId) && x.TenantId == tenantId).ToList();
        }

        public async Task<Point> GetPointByPatruljeAndPost(int PatruljeId, int PostId, string tenantId)
        {
            return _points.FirstOrDefault(x => x.Patrulje.Id.Equals(PatruljeId) && x.Post.Id.Equals(PostId) && x.TenantId == tenantId);
        }

        public async Task<Point> SavePoint(Point point)
        {
            var existing = _points.FirstOrDefault(x => x.Patrulje.Id.Equals(point.Patrulje.Id) && x.Post.Id.Equals(point.Post.Id) && x.TenantId == point.TenantId);
            if (existing != null)
            {
                existing.Points = point.Points;
                existing.Turnout = point.Turnout;
                return existing;
            }

            point.Id = idCount;
            idCount++;
            _points.Add(point);
            return point;
        }

        public async Task AddPoint(Point point)
        {
            point.Id = idCount;
            idCount++;
            _points.Add(point);
        }

        public async Task AddPointRange(List<Point> points)
        {
            _points.AddRange(points);
        }

        public async Task<List<Point>> GetAll(string tenantId)
        {
            return _points.Where(x => x.TenantId == tenantId).ToList();
        }

        public async Task DeletePoint(int Id, string tenantId)
        {
            var point = _points.FirstOrDefault(x => x.Id == Id && x.TenantId == tenantId);
            _points.Remove(point);
        }
    }
    public class PointRepository : IPointRepository
    {
        private readonly IOptions<PointMasterConfig> options;
        private readonly IPatruljeRepository patruljeRepository;
        private readonly IPostRepository postRepository;

        public PointRepository(IOptions<PointMasterConfig> options, IPatruljeRepository patruljeRepository, IPostRepository postRepository)
        {
            this.options = options;
            this.patruljeRepository = patruljeRepository;
            this.postRepository = postRepository;
        }

        private IDbConnection db => new NpgsqlConnection(options.Value.ConnectionString);

        public async Task AddPoint(Point point)
        {
            const string sql = @"
                INSERT INTO points (points, turnout, patrulje_id, post_id, tenant_id)
                VALUES (@point, @turnout, @patruljeId, @postId, @tenantId)
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, new { point = point.Points, turnout = point.Turnout, patruljeId = point.Patrulje.Id, postId = point.Post.Id, tenantId = point.TenantId });
        }

        public async Task AddPointRange(List<Point> points)
        {
            foreach (var point in points)
            {
                await AddPoint(point);
            }
        }

        public async Task DeletePoint(int Id, string tenantId)
        {
            const string sql = @"
                DELETE FROM points
                WHERE id = @id AND tenant_id = @tenantId
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, new { id = Id, tenantId });
        }

        public async Task<Point> GetPointByPatruljeAndPost(int PatruljeId, int PostId, string tenantId)
        {
            const string sql = @"
                SELECT
                    p.id, p.points, p.turnout, p.tenant_id,
                    pa.id, pa.name, pa.tenant_id,
                    po.id, po.name, po.tenant_id
                FROM points p
                INNER JOIN patruljer pa ON p.patrulje_id = pa.id
                INNER JOIN poster po ON p.post_id = po.id
                WHERE p.patrulje_id = @patruljeId AND p.post_id = @postId AND p.tenant_id = @tenantId
                LIMIT 1
            ";
            using var conn = db;

            var result = await conn.QueryAsync<Point, Patrulje, Post, Point>(sql,
            (point, patrulje, post) =>
            {
                point.Patrulje = patrulje;
                point.Post = post;
                return point;
            }, new { patruljeId = PatruljeId, postId = PostId, tenantId }, splitOn: "id, id");

            return result.FirstOrDefault();
        }

        public async Task<Point> SavePoint(Point point)
        {
            var existing = await GetPointByPatruljeAndPost(point.Patrulje.Id, point.Post.Id, point.TenantId);

            using var conn = db;

            if (existing != null)
            {
                const string updateSql = @"
                    UPDATE points
                    SET points = @points,
                        turnout = @turnout
                    WHERE id = @id AND tenant_id = @tenantId
                ";

                await conn.ExecuteAsync(updateSql, new
                {
                    id = existing.Id,
                    points = point.Points,
                    turnout = point.Turnout,
                    tenantId = point.TenantId
                });

                point.Id = existing.Id;
                return point;
            }

            const string insertSql = @"
                INSERT INTO points (points, turnout, patrulje_id, post_id, tenant_id)
                VALUES (@points, @turnout, @patruljeId, @postId, @tenantId)
            ";

            await conn.ExecuteAsync(insertSql, new
            {
                points = point.Points,
                turnout = point.Turnout,
                patruljeId = point.Patrulje.Id,
                postId = point.Post.Id,
                tenantId = point.TenantId
            });

            return point;
        }

        public async Task<List<Point>> GetAll(string tenantId)
        {
            const string sql = @"
                SELECT
                    p.id, p.points, p.turnout, p.tenant_id,
                    pa.id, pa.name, pa.tenant_id,
                    po.id, po.name, po.tenant_id
                FROM points p
                INNER JOIN patruljer pa ON p.patrulje_id = pa.id
                INNER JOIN poster po ON p.post_id = po.id
                WHERE p.tenant_id = @tenantId
            ";
            using var conn = db;

            var result = await conn.QueryAsync<Point, Patrulje, Post, Point>(sql,
            (point, patrulje, post) =>
            {
                point.Patrulje = patrulje;
                point.Post = post;
                return point;
            }, new { tenantId }, splitOn: "id, id");
            return result.ToList();
        }

        public async Task<Point> GetPointById(int Id, string tenantId)
        {
            const string sql = @"
                SELECT
                    *
                FROM points
                WHERE id = @id AND tenant_id = @tenantId
            ";
            using var conn = db;
            return await conn.QueryFirstOrDefaultAsync<Point>(sql, new { id = Id, tenantId });
        }

        public async Task<List<Point>> GetPointByPatrulje(int PatruljeId, string tenantId)
        {
            const string sql = @"
                SELECT
                    p.id, p.points, p.turnout, p.tenant_id,
                    pa.id, pa.name, pa.tenant_id,
                    po.id, po.name, po.tenant_id
                FROM points p
                INNER JOIN patruljer pa ON p.patrulje_id = pa.id
                INNER JOIN poster po ON p.post_id = po.id
                WHERE p.patrulje_id = @patruljeId AND p.tenant_id = @tenantId
            ";
            using var conn = db;

            var result = await conn.QueryAsync<Point, Patrulje, Post, Point>(sql,
            (point, patrulje, post) =>
            {
                point.Patrulje = patrulje;
                point.Post = post;
                return point;
            }, new { patruljeId = PatruljeId, tenantId }, splitOn: "id, id");

            return result.ToList();
        }
    }
}