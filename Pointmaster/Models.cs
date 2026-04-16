namespace Pointmaster
{
    public class Patrulje
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Point
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int Turnout { get; set; }
        public Patrulje Patrulje { get; set; }
        public Post Post { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}