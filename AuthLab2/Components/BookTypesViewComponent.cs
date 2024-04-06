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
            ViewBag.SelectedBookType = RouteData?.Values["bookType"];
            var bookTypes = _bookRepo.Books
                .Select(x => x.Classification)
                .Distinct()
                .OrderBy(x => x);
            return View(bookTypes);
        }
    }
}
