namespace Pointmaster.Repositories
{
    public interface IPointRepository
    {
        Task AddPoint(Point point);
        Task AddPointRange(List<Point> points);
        Task<Point> GetPointById(int Id);
        Task<List<Point>> GetPointByPatrulje(int PatruljeId);
        Task<List<Point>> GetAll();
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
    }
}