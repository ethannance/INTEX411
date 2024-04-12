using AuthLab2.Models;
using AuthLab2.Models.ViewModels;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace AuthLab2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ILegoRepository _repo;
        private readonly InferenceSession _session;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ILogger<AdminController> logger, ILegoRepository repo, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutAdmin() { return View(); }
        [HttpGet]
        public IActionResult OrdersDeleteAdmin(int id) // Delete confirmation view
        {
            var orderToDelete = _repo.Orders
                .Single(x => x.transaction_ID == id);

            return View(orderToDelete);
        }
        [HttpPost]
        public IActionResult OrdersDeleteAdmin(Order orderToDelete) // Double check deletion
        {
            _repo.DeleteOrder(orderToDelete);
            return View("ConfirmationAdmin", orderToDelete);
        }
        public IActionResult OrdersListAdmin()  // Lists some order information
        {
            
            //Linq
            var orders = _repo.Orders
                .Take(50)
                .OrderBy(x => x.transaction_ID)
                .ToList();

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
        [ValidateAntiForgeryToken] //Secure form data
        public IActionResult ProductsEditAdmin(Product updatedProduct) //Saved edits to DB
        {
            _repo.EditProduct(updatedProduct);

            return View("ConfirmationAdmin", updatedProduct);
        }
        public IActionResult UsersListAdmin() 
        {
            var users = _repo.Customers
                .Take(50)
                .OrderBy(x => x.customer_ID)
                .ToList();

            return View(users); 
        }
        [HttpGet]
        public IActionResult UsersListEditAdmin(int id) 
        {
            var userToEdit = _repo.Customers
                .Single(x => x.customer_ID == id);

            return View("UsersListEditAdmin", userToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //Secure form data
        public IActionResult UsersListEditAdmin(Customer updatedCustomer)
        {
            _repo.EditUser(updatedCustomer);

            return View("ConfirmationAdmin", updatedCustomer);
        }
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            // Find the user by id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle user not found
                return NotFound();
            }

            // Check if the user is already in the "Admin" role
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Add the user to the "Admin" role
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            // Remove the user from the "Customer" role (if they are in it)
            if (await _userManager.IsInRoleAsync(user, "Customer"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Customer");
            }

            // Redirect or return appropriate response
            return RedirectToAction("UsersListAdmin");
        }

        [HttpPost]
        public async Task<IActionResult> MakeCustomer(string userId)
        {
            // Find the user by id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle user not found
                return NotFound();
            }

            // Check if the user is already in the "Customer" role
            if (!await _userManager.IsInRoleAsync(user, "Customer"))
            {
                // Add the user to the "Customer" role
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            // Remove the user from the "Admin" role (if they are in it)
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            // Redirect or return appropriate response
            return RedirectToAction("UsersListAdmin");
        }
        [HttpGet] //Delete Customers
        public IActionResult UsersListDeleteAdmin(int id)
        {
            var userToDelete = _repo.Customers
                .Single(x => x.customer_ID == id);

            return View("UsersListDeleteAdmin", userToDelete);
        }
        [HttpPost]
        public IActionResult UsersListDeleteAdmin(Customer customerToDelete)
        {
            _repo.DeleteUser(customerToDelete);

            return View("ConfirmationAdmin", customerToDelete);
        }

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
        [ValidateAntiForgeryToken] //Secure form data
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
