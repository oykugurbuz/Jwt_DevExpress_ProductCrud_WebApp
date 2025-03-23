using Microsoft.AspNetCore.Mvc;

namespace WebAppDemo.Controllers
{
    public class PublicController : Controller
    {
        public IActionResult PublicPage()
        {
            return View();
        }
    }
}
