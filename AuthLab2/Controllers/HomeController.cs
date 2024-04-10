using AuthLab2.Components;
using AuthLab2.Models;
using AuthLab2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Diagnostics;
using System.Drawing.Printing;

namespace AuthLab2.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILegoRepository _repo;
        private readonly InferenceSession _session;

        public HomeController(ILogger<HomeController> logger, ILegoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }



        public IActionResult Index(int pageNum = 1, string? productType = null, int pageSize = 5, string? productColor = null)
        {
            // Validate and set pageSize based on user input or default to 5 if the input is outside the allowed range
            pageSize = (new[] { 5, 10, 20 }).Contains(pageSize) ? pageSize : 5;

            // Fetch all relevant products as a List<Product> initially
            var allProducts = _repo.Products
                .Where(x => (productType == null || x.category == productType) &&
                       (productColor == null || x.primary_color == productColor))
                .OrderBy(x => x.name)
                .ToList(); // This ensures the operation is executed and data is fetched into memory

            var uniqueProductsByName = new HashSet<string>();
            var filteredProducts = new List<Product>();

            // Filter to ensure unique names
            foreach (var product in allProducts)
            {
                if (uniqueProductsByName.Add(product.name))
                {
                    filteredProducts.Add(product);
                }
            }

            // Now, handle pagination on the already filtered list
            var paginatedProducts = filteredProducts
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToList(); // Since we're working with List<Product>, no casting issue should arise

            var viewModel = new ProductsListViewModel
            {
                Products = paginatedProducts.AsQueryable(), // Convert the list to IQueryable

                PaginationInfo = new PaginationInfo
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = pageSize,
                    TotalItems = filteredProducts.Count // Use count of filtered products for total items
                },

                CurrentProductType = productType,
                CurrentProductColor = productColor
            };

            return View(viewModel);
        }

        public IActionResult ProductDetails(int id)
        {
            var product = _repo.Products.FirstOrDefault(x => x.product_ID == id);
            var contentRecs = _repo.content_Recs.FirstOrDefault(r => r.ProductID == id);

            if (product == null || contentRecs == null)
            {
                return NotFound();
            }

            var recommendedProducts = new List<Product>();
            if (!string.IsNullOrEmpty(contentRecs.Recommendation_1))
                recommendedProducts.Add(_repo.Products.FirstOrDefault(x => x.name == contentRecs.Recommendation_1));
            if (!string.IsNullOrEmpty(contentRecs.Recommendation_2))
                recommendedProducts.Add(_repo.Products.FirstOrDefault(x => x.name == contentRecs.Recommendation_2));
            if (!string.IsNullOrEmpty(contentRecs.Recommendation_3))
                recommendedProducts.Add(_repo.Products.FirstOrDefault(x => x.name == contentRecs.Recommendation_3));
            if (!string.IsNullOrEmpty(contentRecs.Recommendation_4))
                recommendedProducts.Add(_repo.Products.FirstOrDefault(x => x.name == contentRecs.Recommendation_4));
            if (!string.IsNullOrEmpty(contentRecs.Recommendation_5))
                recommendedProducts.Add(_repo.Products.FirstOrDefault(x => x.name == contentRecs.Recommendation_5));
            // Repeat for other recommendation properties

            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                content_Recs = contentRecs,
                RecommendedProducts = recommendedProducts
            };

            ViewBag.RefererUrl = Request.Headers["Referer"].ToString();

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secrets()
        {
            return View();
        }

        public IActionResult ProductsAdmin() //Lists all of the products to the admin
        {
            var products = _repo.Products.ToList();

            var productEqualityComparer = new ProductEqualityComparer();
            
            ViewBag.Products = _repo.Products
                .OrderBy(x => x.name)
                .ToList();

            //Linq
            products = products
                .OrderBy(x => x.name)
                .Distinct(productEqualityComparer)
                .ToList();

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
