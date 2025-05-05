using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using WebAppDemo.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppDemo.Controllers
{
    public class JwtResponse
    {
        private string responseString;

        public JwtResponse(string responseString)
        {
            this.AccessToken = responseString;
        }

        public string AccessToken { get; set; }
    }
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [AllowAnonymous] 
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginModel)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("http://localhost:5269/api/Auth/login", loginModel);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya parola.");
                return View(loginModel);
            }

            var responseString = await response.Content.ReadAsStringAsync();

            var jwt = new JwtResponse(responseString);

           
            Response.Cookies.Append("jwt_token", jwt.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return RedirectToAction("SecretPage","Authorize" );
        }

      
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel signupModel)
        {
            var apiUrl = "http://localhost:5269/api/Auth/signup"; 

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


    }
}
