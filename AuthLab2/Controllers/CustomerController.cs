using AuthLab2.Models;
using AuthLab2.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using System.Drawing.Printing;
using System.Security.Claims;

namespace AuthLab2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ILegoRepository _repo;
        private readonly InferenceSession _session;

        public CustomerController(ILogger<CustomerController> logger, ILegoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }
        public IActionResult ProductsCust(int pageNum = 1, string? productType = null, int pageSize = 5, string? productColor = null)
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
        public IActionResult AboutCust()
        {
            return View();
        }
        public IActionResult PrivacyCust()
        {
            return View();
        }
        public IActionResult Index()
        {
            var userId = User.Identity.IsAuthenticated
                         ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "157") // Default to 157 if parsing fails
                         : 157; // Default user ID for recommendations if not logged in

            var userRecs = _repo.user_recommendations.FirstOrDefault(ur => ur.customer_ID == userId)
                           ?? _repo.user_recommendations.FirstOrDefault(ur => ur.customer_ID == 157); // Fallback to default user recommendations

            var viewModel = new TestViewModel
            {
                // Based on either the user's recommendations or the default
                user_recommendations = userRecs,
                Product = _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.if_you_liked),
                uRecommendedProducts = new List<Product>
        {
            _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.Recommendation_1),
            _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.Recommendation_2),
            _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.Recommendation_3),
            _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.Recommendation_4),
            _repo.Products.FirstOrDefault(p => p.product_ID == userRecs.Recommendation_5)
        }.Where(p => p != null).ToList()
            };

            ViewBag.RefererUrl = Request.Headers["Referer"].ToString();

            return View(viewModel);
        }

        public IActionResult ProductDetailsCust(int id, string returnUrl = null)
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
    }
}
