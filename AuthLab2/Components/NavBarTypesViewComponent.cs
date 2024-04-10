using Microsoft.AspNetCore.Mvc;

namespace AuthLab2.Components
{
    public class NavBarTypesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string user)
        {
            return View(user);
        }
    }
}
