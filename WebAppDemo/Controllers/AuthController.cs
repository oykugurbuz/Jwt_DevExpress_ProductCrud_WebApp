using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebAppDemo.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppDemo.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public AuthController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [AllowAnonymous] 
        [HttpPost]
        public async Task<IActionResult> Login(User loginModel)
        {
            var apiUrl = "http://localhost:5269/api/Auth/login";
            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<dynamic>(result).token;

               
                TempData["Token"] = token;

              
                return RedirectToAction("Index", "Home");
            }

            
            ViewBag.ErrorMessage = "Yanlış kullanıcı adı veya şifre.";
            return View();
        }

      
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel signupModel)
        {
            
            var apiUrl = "https://localhost:5269/api/Auth/signup"; 

            var client = _clientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(signupModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("login", "auth");
            }
            ViewBag.ErrorMessage = "Kayıt işlemi başarısız. Kullanıcı adı zaten alınmış olabilir.";
            return View();
        }

        public IActionResult Logout()
        {
            TempData.Remove("Token");
            return RedirectToAction("login", "auth");
        }


    }
}
