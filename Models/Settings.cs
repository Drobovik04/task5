namespace task5.Models
{
    public class Settings
    {
        public string Language { get; set; } = "en";
        public int Seed { get; set; } = 0;
        public double AvgLikes { get; set; } = 5.0;
        public double AvgReviews { get; set; } = 5.0;
    }
}
