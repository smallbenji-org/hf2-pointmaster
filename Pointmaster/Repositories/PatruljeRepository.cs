namespace Pointmaster.Repositories
{
    public interface IPatruljeRepository
    {
        void AddPatrulje(Patrulje data);
        void AddPatruljeRange(List<Patrulje> data);
        Patrulje GetPatruljeById(int Id);
        List<Patrulje> GetAll();
    }

    public class DummyPatruljeRepository : IPatruljeRepository
    {
        private List<Patrulje> _patruljes = [];

        public Patrulje GetPatruljeById(int Id)
        {
            return _patruljes.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public void AddPatrulje(Patrulje data)
        {
            _patruljes.Add(data);
        }

        public void AddPatruljeRange(List<Patrulje> data)
        {
            _patruljes.AddRange(data);
        }

        public List<Patrulje> GetAll()
        {
            return _patruljes;
        }
    }
}