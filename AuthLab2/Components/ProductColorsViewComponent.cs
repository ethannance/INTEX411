using AuthLab2.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthLab2.Components
{
    public class ProductColorsViewComponent : ViewComponent
    {
        private ILegoRepository _productRepo;

        //Constructor

        public ProductColorsViewComponent(ILegoRepository temp)
        {
            _productRepo = temp;
        }
        public IViewComponentResult Invoke()
        {
            // Attempt to retrieve the bookType from the route values first.
            string productColor = RouteData?.Values["productColors"]?.ToString();

            // If not found, try to get it from the query string.
            if (string.IsNullOrEmpty(productColor))
            {
                productColor = HttpContext.Request.Query["productColors"];
            }

            ViewBag.SelectedProductColor = productColor;

            var productColors = _productRepo.Products
                .Select(x => x.primary_color)
                .Distinct()
                .OrderBy(x => x);

            return View(productColors);
        }

    }
}
