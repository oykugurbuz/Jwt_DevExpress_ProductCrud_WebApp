using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace WebAppDemo.Helpers
{
    public class JwtHelper
    {
        public static string GetTokenFromCookie(HttpContext context)
        {
            return context.Request.Cookies["jwt_token"];
        }

        public static string GetUserNameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
                return "Token çözümlenemedi";

            var userNameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            return userNameClaim?.Value ?? "Kullanıcı Adı Bulunamadı";
        }
    }
}
