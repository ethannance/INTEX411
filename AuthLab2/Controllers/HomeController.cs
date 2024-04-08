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
        private readonly ILegoRepository _repo;

        // This single constructor takes both the logger and repository as parameters
        public HomeController(ILogger<HomeController> logger, ILegoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index(int pageNum = 1, string? productType = null)
        {
            int pageSize = 3;

            var viewModel = new ProductsListViewModel
            {
                Products = _repo.Products
                    .Where(x => productType == null || x.img_link == productType)
                    .OrderBy(x => x.name)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize),

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = (productType == null)
                                  ? _repo.Products.Count()
                                  : _repo.Products.Count(x => x.img_link == productType)
                },

                CurrentProductType = productType
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
