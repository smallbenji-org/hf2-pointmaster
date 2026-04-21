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
        Task<Point> GetPointById(int Id);
        Task<List<Point>> GetPointByPatrulje(int PatruljeId);
        Task<List<Point>> GetAll();
        Task DeletePoint(int Id);
    }

    public class DummyPointRepository : IPointRepository
    {
        private List<Point> _points = [];
        private int idCount = 0;

        public async Task<Point> GetPointById(int Id)
        {
            return _points.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task<List<Point>> GetPointByPatrulje(int PatruljeId)
        {
            return _points.Where(x => x.Patrulje.Id.Equals(PatruljeId)).ToList();
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

        public async Task<List<Point>> GetAll()
        {
            return _points;
        }

        public async Task DeletePoint(int Id)
        {
            var point = _points.FirstOrDefault(x => x.Id == Id);
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
                INSERT INTO points (points, turnout, patrulje_id, post_id)
                VALUES (@point, @turnout, @patruljeId, @postId)
            ";
            using var conn = db;
            await db.ExecuteAsync(sql, new { point = point.Points, turnout = point.Turnout, patruljeId = point.Patrulje.Id, postId = point.Post.Id });
        }

        public async Task AddPointRange(List<Point> points)
        {
            foreach (var point in points)
            {
                await AddPoint(point);
            }
        }

        public async Task DeletePoint(int Id)
        {
            const string sql = @"
                DELETE FROM points
                WHERE id = @id
            ";
            using var conn = db;
            await db.ExecuteAsync(sql, new { id = Id });
        }

        public async Task<List<Point>> GetAll()
        {
            const string sql = @"
                SELECT
                    p.id, p.points, p.turnout,
                    pa.id, pa.name,
                    po.id, po.name
                FROM points p
                INNER JOIN patruljer pa ON p.patrulje_id = pa.id
                INNER JOIN poster po ON p.post_id = po.id
            ";
            using var conn = db;

            var result = await db.QueryAsync<Point, Patrulje, Post, Point>(sql,
            (point, patrulje, post) =>
            {
                point.Patrulje = patrulje;
                point.Post = post;
                return point;
            }, splitOn: "id, id");
            return result.ToList();
        }

        public async Task<Point> GetPointById(int Id)
        {
            const string sql = @"
                SELECT
                    *
                FROM points
                WHERE id = @id
            ";
            using var conn = db;
            return await db.QueryFirstAsync<Point>(sql);
        }

        public async Task<List<Point>> GetPointByPatrulje(int PatruljeId)
        {
            const string sql = @"
                SELECT
                    *
                FROM points
                WHERE patrulje_id = @id
            ";
            using var conn = db;
            return (await db.QueryAsync<Point>(sql, new { id = PatruljeId })).ToList();
        }
    }
}