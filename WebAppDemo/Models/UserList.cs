using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class UserList
    {
        public int Id { get; set; }

        public string? UserName { get; set; } //username


        public long? IdentityNumber { get; set; }
        public int AuthorityLevel { get; set; }
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        public string AuthorityLevelDescription
        {
            get
            {
                return AuthorityLevel switch
                {
                    1 => "Tam yetkili kullanıcı",
                    2 => "2. yetkili kullanıcı",
                    3 => "3. yetkili kullanıcı",
                    4 => "4. yetkili kullanıcı",
                    _ => "Bu kullanıcıya henüz yetki atanmamış."
                };
            } 
        }
        public bool ShowIdentityNumber => AuthorityLevel == 1;
    }
}
    
