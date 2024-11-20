using Bogus;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Text;
using task5.Models;

namespace task5.Services
{
    public class BookGeneratorService
    {
        public List<Book> GenerateBooks(Settings settings, int batch, int batchSize, int startIndex)
        {
            var wordSets = new Dictionary<string, string[]>
            {
                ["en"] = new[] { "time", "dream", "legend", "ocean", "sky", "land", "journey" },
                ["de"] = new[] { "Zeit", "Traum", "Legende", "Ozean", "Himmel", "Land", "Reise" },
                ["fr"] = new[] { "temps", "rêve", "légende", "océan", "ciel", "terre", "voyage" }
            };

            var grammarTemplates = new Dictionary<string, string[]>
            {
                ["en"] = new[]
                {
                    "The Mystery of {0} in {1}",
                    "Journey to the {0} of {1}",
                    "{0} and {1}: A New Chapter",
                    "Forgotten {0} in {1}",
                    "{0} Among the {1}"
                },
                ["de"] = new[]
                {
                    "Das Geheimnis von {0} in {1}",
                    "Reise zum {0} von {1}",
                    "{0} und {1}: Ein neues Kapitel",
                    "Vergessene {0} in {1}",
                    "{0} unter den {1}"
                },
                ["fr"] = new[]
                {
                    "Le mystère de {0} dans {1}",
                    "Voyage au {0} de {1}",
                    "{0} et {1} : un nouveau chapitre",
                    "{0} oubliés dans {1}",
                    "{0} parmi les {1}"
                }
            };
            var faker = new Faker<Book>(locale: settings.Language).UseSeed(settings.Seed + batch);
           
            var books = new List<Book>();
            faker.RuleFor(o => o.Index, s => batch * batchSize + startIndex++)
                .RuleFor(o => o.ISBN, s => s.Random.Replace("###-#-###-#####-#"))
                .RuleFor(o => o.Title, s => 
                {
                    var cultureInfo = new CultureInfo(settings.Language);
                    var textInfo = cultureInfo.TextInfo;
                    var words = wordSets[settings.Language];
                    var templates = grammarTemplates[settings.Language];

                    var template = s.PickRandom(templates);
                    var word1 = s.PickRandom(words);
                    var word2 = s.PickRandom(words);
                    return textInfo.ToTitleCase(string.Format(template, word1, word2));
                })
                .RuleFor(o => o.Author, s => s.Name.FullName())
                .RuleFor(o => o.Publisher, s => s.Company.CompanyName() + ", " + s.Date.BetweenDateOnly(new DateOnly(1900, 1, 1), new DateOnly(2024, 11, 1)).Year)
                .RuleFor(o => o.Likes, s => settings.AvgLikes % 1 != 0 && s.Random.Double(0, 1) <= settings.AvgLikes % 1 ? (int)settings.AvgLikes/1 + 1 : (int)settings.AvgLikes/1)
                .RuleFor(o => o.CoverImage, s => s.Image.PicsumUrl())
                .RuleFor(o => o.ReviewDetails, s => GenerateReviews(s, (settings.AvgReviews % 1 != 0 && s.Random.Double(0, 1) <= settings.AvgReviews % 1 ? (int)settings.AvgReviews/1 + 1 : (int)settings.AvgReviews/1)));

            books.AddRange(faker.Generate(batchSize));

            return books;
        }

        private List<Review> GenerateReviews(Bogus.Faker faker, int reviewsCount)
        {
            var reviewPhrases = new Dictionary<string, string[]>
            {
                ["en"] = new[]
                {
                    "An absolutely fantastic read!",
                    "Couldn't put this book down. Highly recommended!",
                    "A bit slow at times, but overall a great story.",
                    "The characters felt so real and relatable.",
                    "A wonderful book with unexpected twists.",
                    "The plot was intriguing, but the ending was predictable."
                },
                ["de"] = new[]
                {
                    "Ein absolut fantastisches Buch!",
                    "Konnte das Buch nicht aus der Hand legen. Sehr zu empfehlen!",
                    "Etwas langsam an manchen Stellen, aber insgesamt eine tolle Geschichte.",
                    "Die Charaktere fühlten sich so real und nachvollziehbar an.",
                    "Ein wunderbares Buch mit unerwarteten Wendungen.",
                    "Die Handlung war faszinierend, aber das Ende vorhersehbar."
                },
                ["fr"] = new[]
                {
                    "Un livre absolument fantastique !",
                    "Impossible de poser ce livre. Hautement recommandé !",
                    "Un peu lent par moments, mais globalement une belle histoire.",
                    "Les personnages semblaient si réels et attachants.",
                    "Un merveilleux livre avec des rebondissements inattendus.",
                    "L'intrigue était captivante, mais la fin était prévisible."
                }
            };

            var reviewTemplates = new Dictionary<string, string[]>
            {
                ["en"] = new[]
                {
                    "{0} {1}",
                    "{0} {1} {2}",
                    "Overall, {0} {1}. {2}",
                    "{0}. I especially loved {1}.",
                    "While {0}, {1}. {2}"
                },
                ["de"] = new[]
                {
                    "{0} {1}",
                    "{0} {1} {2}",
                    "Insgesamt {0} {1}. {2}",
                    "{0}. Besonders mochte ich {1}.",
                    "Während {0}, {1}. {2}"
                },
                ["fr"] = new[]
                {
                    "{0} {1}",
                    "{0} {1} {2}",
                    "Dans l'ensemble, {0} {1}. {2}",
                    "{0}. J'ai particulièrement aimé {1}.",
                    "Bien que {0}, {1}. {2}"
                }
            };

            var reviews = new List<Review>();

            for (int i = 0; i < reviewsCount; i++)
            {
                var review = new Review();
                review.Reviewer = faker.Name.FullName();
                var phrases = reviewPhrases[faker.Locale];
                var templates = reviewTemplates[faker.Locale];

                var randomPhrases = faker.Random.ListItems(phrases, faker.Random.Int(2, 4));
                var template = faker.PickRandom(templates);

                review.ReviewText = string.Format(template, randomPhrases[0], randomPhrases[1], randomPhrases.Count > 2 ? randomPhrases[2] : "");
                reviews.Add(review);
            }

            return reviews;
        }
    }
}
