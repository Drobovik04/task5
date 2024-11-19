namespace task5.Models
{
    public class Book
    {
        public int Index { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public double Likes { get; set; }
        public double Reviews { get; set; }
        public string CoverImage { get; set; }
        public List<Review> ReviewDetails { get; set; }
    }
    public class Review
    {
        public string Reviewer { get; set; }
        public string ReviewText { get; set; }
    }
}
