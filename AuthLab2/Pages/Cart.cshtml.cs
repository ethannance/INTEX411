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

        public IActionResult OnPost(int productId, string returnUrl, bool removeItem = false)
        {
            Product p = _repo.Products.FirstOrDefault(x => x.product_ID == productId);

            if (p != null)
            {
                Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();

                if (removeItem)
                {
                    // Find the cart line for the specified product
                    var line = Cart.Lines.FirstOrDefault(l => l.Product.product_ID == productId);
                    if (line != null)
                    {
                        if (line.Quantity > 1)
                        {
                            // If quantity is greater than 1, decrease it by 1
                            Cart.DecreaseItemQuantity(p, 1); // Make sure this method exists and correctly updates the quantity in your Cart class
                        }
                        else
                        {
                            // If quantity is 1, remove the item from the cart
                            Cart.RemoveLine(p);
                        }
                    }
                }
                else
                {
                    Cart.AddItem(p, 1);
                }

                HttpContext.Session.SetJson("cart", Cart);
            }

            return RedirectToPage(new { returnUrl = returnUrl });
        }





    }
}

