using AuthLab2.Models;
using AuthLab2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AuthLab2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookRepository _repo;

        // This single constructor takes both the logger and repository as parameters
        public HomeController(ILogger<HomeController> logger, IBookRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index(int pageNum = 1, string? bookType = null)
        {
            int pageSize = 3;

            var viewModel = new BooksListViewModel
            {
                Books = _repo.Books
                    .Where(x => bookType == null || x.Classification == bookType)
                    .OrderBy(x => x.Title)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = (bookType == null)
                                  ? _repo.Books.Count()
                                  : _repo.Books.Count(x => x.Classification == bookType)
                },

                CurrentBookType = bookType
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secrets()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
