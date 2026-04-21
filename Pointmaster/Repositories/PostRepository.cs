namespace Pointmaster.Repositories
{
    public interface IPostRepository
    {
        void AddPost(Post data);
        Post GetPostById(int Id);
        List<Post> GetAll();
    }

    public class DummyPostRepository : IPostRepository
    {
        private List<Post> _posts = [];
        private int idCount = 0;

        public Post GetPostById(int Id)
        {
            return _posts.FirstOrDefault(x => x.Id.Equals(Id));
        }

        public void AddPost(Post data)
        {
            data.Id = idCount;
            idCount++;
            _posts.Add(data);
        }

        public List<Post> GetAll()
        {
            return _posts;
        }
    }
}