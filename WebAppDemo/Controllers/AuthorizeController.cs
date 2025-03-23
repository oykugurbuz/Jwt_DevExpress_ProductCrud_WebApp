using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppDemo.Controllers
{
    [Authorize]
    public class AuthorizeController : Controller
    {
        public IActionResult SecretPage()
        {
            return View();
        }
    }
}
