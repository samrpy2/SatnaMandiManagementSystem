using System;
using System.ComponentModel.DataAnnotations;

namespace UserLogin.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        // 👤 किस यूजर ने काम किया (Email)
        public string UserEmail { get; set; } = string.Empty;

        // ⚡ क्या काम किया (CREATE, EDIT, SOFT-DELETE)
        public string Action { get; set; } = string.Empty;

        // 📦 किस आइटम पर काम किया और क्या बदलाव किया
        public string Details { get; set; } = string.Empty;

        // ⏱️ किस तारीख और समय पर किया
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
