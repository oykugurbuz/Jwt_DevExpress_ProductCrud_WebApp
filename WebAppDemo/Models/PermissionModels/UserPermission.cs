namespace WebAppDemo.Models.PermissionModels
{
    public class UserPermission
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Foreign Key to AppUserInfo
        public AppUserInfo? User { get; set; } // Navigation Property to AppUserInfo //Yetkilenen kullanıcı dotne

        public int PermissionId { get; set; } // Foreign Key to Permission

        public Permission? Permission { get; set; } // Navigation Property to Permission

        public int GivenByUserId { get; set; }  // Yetkiyi veren kullanıcı
        public AppUserInfo GivenByUser { get; set; }

        public DateTime GivenDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
