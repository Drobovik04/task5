using Microsoft.AspNetCore.Mvc;
using task5.Models;
using task5.Services;
using System.Text.Json.Serialization;

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
    }
}
