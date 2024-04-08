using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthLab2.Infrastructure;
using AuthLab2.Models; // Make sure this using directive matches the namespace of your Cart class

namespace AuthLab2.Pages
{
    public class CartModel : PageModel
    {
        private ILegoRepository _repo;

        public CartModel(ILegoRepository temp)
        {
            _repo = temp;
        }
        public Cart? Cart { get; set; }
        public string ReturnUrl { get; set; } = "/";

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
       
        }

        public IActionResult OnPost(int productId, string returnUrl)
        {
            Product b = _repo.Products
                .FirstOrDefault(x => x.product_ID == productId);

            if (b != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
                Cart.AddItem(b, 1);
                HttpContext.Session.SetJson("cart", Cart);
            }

            return RedirectToPage (new {returnUrl = returnUrl});
        }
    }
}

