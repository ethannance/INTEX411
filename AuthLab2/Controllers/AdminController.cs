using Microsoft.AspNetCore.Mvc;

namespace AuthLab2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
