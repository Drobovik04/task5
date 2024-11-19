using Microsoft.AspNetCore.Mvc;
using task5.Models;
using task5.Services;
using System.Text.Json.Serialization;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace task5.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookGeneratorService _bookGenerator;
        public BooksController()
        {
            _bookGenerator = new BookGeneratorService();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var settings = new Settings();
            return View(settings);
        }
        [HttpPost]
        public JsonResult GenerateBooks([FromBody] Settings settings, int batch, int startIndex)
        {
            var books = _bookGenerator.GenerateBooks(settings, batch, 20, startIndex);
            return Json(books);
        }
        public IActionResult ExportToCsv([FromBody] Settings settings, int totalbatches)
        {
            var allBooks = new List<Book>();
            for (int batch = 0; batch < totalbatches; batch++)
            {
                var books = _bookGenerator.GenerateBooks(settings, batch, 20, 0);
                allBooks.AddRange(books);
            }

            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.WriteRecords(allBooks); 
            writer.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream, "text/csv", "books.csv");
        }
    }
}
