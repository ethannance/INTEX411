using Microsoft.AspNetCore.Mvc;
using AuthLab2.Models;

namespace AuthLab2.Components
{
    public class ProductTypesViewComponent : ViewComponent
    {
        private ILegoRepository _productRepo;

        //Constructor

        public ProductTypesViewComponent(ILegoRepository temp) 
        {
            _productRepo = temp;
        }
        public IViewComponentResult Invoke()
        {
            // Attempt to retrieve the bookType from the route values first.
            string productType = RouteData?.Values["productType"]?.ToString();

            // If not found, try to get it from the query string.
            if (string.IsNullOrEmpty(productType))
            {
                productType = HttpContext.Request.Query["productType"];
            }

            ViewBag.SelectedProductType = productType;

            var productTypes = _productRepo.Products
                .Select(x => x.category)
                .Distinct()
                .OrderBy(x => x);

            return View(productTypes);
        }

    }
}
