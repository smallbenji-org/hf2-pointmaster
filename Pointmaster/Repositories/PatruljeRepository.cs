using System.Data;
using System.Formats.Cbor;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Pointmaster.Repositories
{
    public interface IPatruljeRepository
    {
        Task AddPatrulje(Patrulje data);
        Task AddPatruljeRange(List<Patrulje> data);
        Task<Patrulje> GetPatruljeById(int Id);
        Task<Patrulje> GetPatruljeById(int Id, string tenantId);
        Task<List<Patrulje>> GetAll(string tenantId);
        Task DeletePatrulje(int Id, string tenantId);
    }

    public class DummyPatruljeRepository : IPatruljeRepository
    {
        private List<Patrulje> _patruljes = [];
        private int idCount = 0;

        public async Task<Patrulje> GetPatruljeById(int Id, string tenantId)
        {
            return _patruljes.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task<Patrulje> GetPatruljeById(int Id)
        {
            return _patruljes.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public async Task AddPatrulje(Patrulje data)
        {
            data.Id = idCount;
            idCount++;
            _patruljes.Add(data);
        }

        public async Task AddPatruljeRange(List<Patrulje> data)
        {
            _patruljes.AddRange(data);
        }

        public async Task<List<Patrulje>> GetAll(string tenantId)
        {
            return _patruljes.Where(x => x.TenantId == tenantId).ToList();
        }

        public async Task DeletePatrulje(int Id, string tenantId)
        {
            var patrulje = _patruljes.FirstOrDefault(x => x.Id == Id && x.TenantId == tenantId);

            _patruljes.Remove(patrulje);
        }
    }

    public class PatruljeRepository : IPatruljeRepository
    {
        private readonly IOptions<PointMasterConfig> options;

        public PatruljeRepository(IOptions<PointMasterConfig> options)
        {
            this.options = options;
        }

        IDbConnection db => new NpgsqlConnection(options.Value.ConnectionString);

        public async Task AddPatrulje(Patrulje data)
        {
            const string sql = @"
            INSERT INTO patruljer (name, tenant_id)
            VALUES (@Name, @TenantId)
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, data);
        }

        public async Task AddPatruljeRange(List<Patrulje> data)
        {
            foreach (var patrulje in data)
            {
                await AddPatrulje(patrulje);
            }
        }

        public async Task<List<Patrulje>> GetAll(string tenantId)
        {
            const string sql = @"
            SELECT
                *
            FROM patruljer
            WHERE tenant_id = @tenantId
            ";
            using var conn = db;
            return (await conn.QueryAsync<Patrulje>(sql, new { tenantId })).ToList();
        }

        public async Task<Patrulje> GetPatruljeById(int Id, string tenantId)
        {
            const string sql = @"
            SELECT
                *
            FROM patruljer
            WHERE id = @Id AND tenant_id = @tenantId
            ";
            using var conn = db;
            return await conn.QueryFirstOrDefaultAsync<Patrulje>(sql, new { id = Id, tenantId });
        }

        public async Task<Patrulje> GetPatruljeById(int Id)
        {
            const string sql = @"
            SELECT
                *
            FROM patruljer
            WHERE id = @Id
            ";
            using var conn = db;
            return await conn.QueryFirstOrDefaultAsync<Patrulje>(sql, new { id = Id });
        }

        public async Task DeletePatrulje(int Id, string tenantId)
        {
            const string sql = @"
            DELETE FROM patruljer
            WHERE id = @id AND tenant_id = @tenantId
            ";
            using var conn = db;
            await conn.ExecuteAsync(sql, new { id = Id, tenantId });
        }
    }
}