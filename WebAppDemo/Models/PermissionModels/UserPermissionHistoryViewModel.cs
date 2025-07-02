namespace WebAppDemo.Models.PermissionModels
{
    public class UserPermissionHistoryViewModel
    {
        public string? User { get; set; }

        public string? Module { get; set; }

        public string? Permission { get; set; }

        public string? GivenBy { get; set; }
        public DateTime? GivenDate { get; set; }

        public bool IsActive { get; set; }

        public string? RevokedByUser { get; set; }

        public DateTime? RevokedDate { get; set; }
    }
}
