namespace Pointmaster
{
    public class Patrulje
    {
        public Patrulje() {}

        public Patrulje(string name)
        {
            this.Name = name;
        }

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
        public Post() {}
        public Post(string name) { this.Name = name; }

        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PointMasterConfig
    {
        public string ConnectionString { get; set; }
    }
}