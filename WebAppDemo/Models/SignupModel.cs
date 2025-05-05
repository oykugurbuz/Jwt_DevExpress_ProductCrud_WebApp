using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class SignupModel
    {
        public int Id { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public string? Password { get; set; }
    }
}
