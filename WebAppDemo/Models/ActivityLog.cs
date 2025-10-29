using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class ActivityLog
    {
        [Key]
        public int ActivityId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string? PageUrl { get; set; }

        public string? ActionName { get; set; }

        public string? ControllerName { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DurationMs { get; set; }

        public string? CliendIp { get; set; }

        public string? BrowserInfo { get; set; }
    }
}
