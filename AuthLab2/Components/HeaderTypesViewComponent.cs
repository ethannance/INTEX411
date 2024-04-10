using Microsoft.AspNetCore.Mvc;

namespace AuthLab2.Components
{
    public class HeaderTypesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string title)
        {
            return View("Default", title);
        }
    }
}
