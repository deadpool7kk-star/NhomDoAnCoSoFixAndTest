using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Book2.Models
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? HoTen { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }
        
        public DateTime? NgaySinh { get; set; }
    }
}
