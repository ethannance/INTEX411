using Microsoft.AspNetCore.Mvc;

namespace AuthLab2.Components
{
    public class SubmenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string view)
        {
            return View(view);
        }
    }
}
