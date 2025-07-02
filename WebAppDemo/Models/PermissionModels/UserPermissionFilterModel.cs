using System.Text.Json.Serialization;

namespace WebAppDemo.Models.PermissionModels
{
    public class UserPermissionFilterModel
    {
        [JsonPropertyName("UserName")]
        public string? UserName { get; set; }
        [JsonPropertyName("ModuleName")]
        public string? ModuleName { get; set; }
        [JsonPropertyName("PermissionName")]
        public string? PermissionName { get; set; }
        [JsonPropertyName("GivenBy")]
        public string? GivenBy { get; set; }
        [JsonPropertyName("GivenDateStart")]
        public DateTime? GivenDateStart { get; set; }
        [JsonPropertyName("GivenDateEnd")]
        public DateTime? GivenDateEnd { get; set; }
        [JsonPropertyName("IsActive")]
        public bool? IsActive { get; set; }
        [JsonPropertyName("RevokedByUser")]
        public string? RevokedByUser { get; set; }
        [JsonPropertyName("RevokedDateStart")]
        public DateTime? RevokedDateStart { get; set; }
        [JsonPropertyName("RevokedDateEnd")]
        public DateTime?  RevokedDateEnd { get; set; }
    }
}
