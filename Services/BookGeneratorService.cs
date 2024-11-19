using Bogus;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using task5.Models;

namespace task5.Services
{
    public class BookGeneratorService
    {
        public List<Book> GenerateBooks(Settings settings, int batch, int batchSize, int startIndex)
        {
            var faker = new Faker<Book>(locale: settings.Language).UseSeed(settings.Seed + batch);
            var books = new List<Book>();
            faker.RuleFor(o => o.Index, s => batch * batchSize + startIndex++)
                .RuleFor(o => o.ISBN, s => s.Random.Replace("###-#-###-#####-#"))
                .RuleFor(o => o.Title, s => s.Address.FullAddress())
                .RuleFor(o => o.Author, s => s.Name.FullName())
                .RuleFor(o => o.Publisher, s => s.Company.CompanyName() + ", " + s.Date.BetweenDateOnly(new DateOnly(1900, 1, 1), new DateOnly(2024, 11, 1)).Year)
                .RuleFor(o => o.Likes, s => settings.AvgLikes % 1 != 0 && s.Random.Double(0, 1) <= settings.AvgLikes % 1 ? (int)settings.AvgLikes/1 + 1 : (int)settings.AvgLikes/1)
                .RuleFor(o => o.CoverImage, s => s.Image.PicsumUrl())
                .RuleFor(o => o.ReviewDetails, s => GenerateReviews(s, (settings.AvgReviews % 1 != 0 && s.Random.Double(0, 1) <= settings.AvgReviews % 1 ? (int)settings.AvgReviews/1 + 1 : (int)settings.AvgReviews/1)));

            books.AddRange(faker.Generate(batchSize));

            return books;
        }

        private List<Review> GenerateReviews(Faker faker, int reviewsCount)
        {
            var reviews = new List<Review>();

            for (int i = 0; i < reviewsCount; i++)
            {
                reviews.Add(new Review() { Reviewer = faker.Name.FullName(), ReviewText = faker.Address.FullAddress()});
            }

            return reviews;
        }
    }
}
