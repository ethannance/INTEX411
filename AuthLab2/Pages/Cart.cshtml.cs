using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AuthLab2.Infrastructure;
using AuthLab2.Models; // Make sure this using directive matches the namespace of your Cart class

namespace AuthLab2.Pages
{
    public class CartModel : PageModel
    {
        private IBookRepository _repo;

        public CartModel(IBookRepository temp)
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

        public IActionResult OnPost(int bookId, string returnUrl)
        {
            Book b = _repo.Books
                .FirstOrDefault(x => x.BookId == bookId);

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

