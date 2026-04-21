namespace Pointmaster.Repositories
{
    public interface IPointRepository
    {
        void AddPoint(Point point);
        void AddPointRange(List<Point> points);
        Point GetPointById(int Id);
        List<Point> GetPointByPatrulje(int PatruljeId);
    }

    public class DummyPointRepository : IPointRepository
    {
        private List<Point> _points = [];
        private int idCount = 0;

        public Point GetPointById(int Id)
        {
            return _points.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public List<Point> GetPointByPatrulje(int PatruljeId)
        {
            return _points.Where(x => x.Patrulje.Id.Equals(PatruljeId)).ToList();
        }

        public void AddPoint(Point point)
        {
            point.Id = idCount;
            idCount++;
            _points.Add(point);
        }

        public void AddPointRange(List<Point> points)
        {
            _points.AddRange(points);
        }
    }
}