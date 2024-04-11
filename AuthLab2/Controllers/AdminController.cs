using AuthLab2.Models;
using AuthLab2.Models.ViewModels;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;

namespace AuthLab2.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ILegoRepository _repo;
        private readonly InferenceSession _session;

        public AdminController(ILogger<AdminController> logger, ILegoRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutAdmin() { return View(); }
        [HttpGet]
        public IActionResult OrderDeleteAdmin(int id) // Delete confirmation view
        {
            var orderToDelete = _repo.Orders
                .Single(x => x.transaction_ID == id);

            return View(orderToDelete);
        }
        [HttpPost]
        public IActionResult OrdersDeleteAdmin(Order orderToDelete)
        {
            _repo.DeleteOrder(orderToDelete);
            return View("ConfirmationAdmin", orderToDelete);
        }
        public IActionResult OrdersListAdmin()  // Lists some order information
        {
            
            //Linq
            var orders = _repo.Orders
                .OrderBy(x => x.transaction_ID)
                .ToList();

            //var orderViewModels = orders.Select(o => new OrderViewModel
            //{
            //    transaction_ID = o.transaction_ID,
            //    date = o.date,
            //    amount = (int)o.amount,
            //    shipping_address = o.shipping_address,
            //    fraud = o.fraud
            //}).ToList();

            return View(orders);
        }
        public IActionResult PrivacyAdmin() { return View(); }
        [HttpGet]
        public IActionResult ProductsDeleteAdmin(int id)  // Delete confirmation view
        { 
            var productToDelete = _repo.Products
                .Single(x => x.row_ID == id);

            return View("ProductsDeleteAdmin", productToDelete); 
        }
        [HttpPost]
        public IActionResult ProductsDeleteAdmin(Product productToDelete) // Double checks deletion
        {
            _repo.DeleteProduct(productToDelete);

            return View("ConfirmationAdmin", productToDelete);
        }
        [HttpGet]
        public IActionResult ProductsEditAdmin(int id)  //Edit Product view
        { 
            var productToEdit = _repo.Products
                .Single(x => x.row_ID == id);

            return View("ProductsEditAdmin", productToEdit); 
        }
        [HttpPost]
        public IActionResult ProductsEditAdmin(Product updatedProduct)
        {
            _repo.EditProduct(updatedProduct);

            return View("ConfirmationAdmin", updatedProduct);
        }
        public IActionResult UsersListAdmin() { return View(); }
        public IActionResult UsersListEditAdmin() { return View(); }
        public IActionResult UsersListEditConfirmation() { return View(); }
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
        [HttpGet]
        public IActionResult ProductsAddAdmin() //New Product view
        {
            return View(new Product());
        }
        [HttpPost]
        public IActionResult ProductsAddAdmin(Product response) //Add the new product
        {
            if (ModelState.IsValid)
            {
                _repo.AddProduct(response);

                return View("ConfirmationAdmin", response);
            }
            else
            {
                return View(response);
            }
        }
    }
}
