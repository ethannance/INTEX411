using Microsoft.AspNetCore.Mvc;
using AuthLab2.Models;

namespace AuthLab2.Components
{
    public class BookTypesViewComponent : ViewComponent
    {
        private IBookRepository _bookRepo;

        //Constructor

        public BookTypesViewComponent(IBookRepository temp) 
        {
            _bookRepo = temp;
        }
        public IViewComponentResult Invoke()
        {
            // Attempt to retrieve the bookType from the route values first.
            string bookType = RouteData?.Values["bookType"]?.ToString();

            // If not found, try to get it from the query string.
            if (string.IsNullOrEmpty(bookType))
            {
                bookType = HttpContext.Request.Query["bookType"];
            }

            ViewBag.SelectedBookType = bookType;

            var bookTypes = _bookRepo.Books
                .Select(x => x.Classification)
                .Distinct()
                .OrderBy(x => x);

            return View(bookTypes);
        }

    }
}
