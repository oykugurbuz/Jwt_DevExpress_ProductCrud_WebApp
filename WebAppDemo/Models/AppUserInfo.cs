﻿using System.ComponentModel.DataAnnotations;

namespace WebAppDemo.Models
{
    public class AppUserInfo
    {
        public int Id { get; set; }
        public string? UserName { get; set; } //username
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? UserTypeName { get; set; } //admin-user vs

        public bool IsActive { get; set; }
        public int FailedAttempt { get; set; } //başarısız giriş denemeleri

        public DateTime? LastLoginDate { get; set; }

        public bool RememberMe { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
