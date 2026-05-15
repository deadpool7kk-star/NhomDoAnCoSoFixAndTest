using System.ComponentModel.DataAnnotations;

namespace Book2.Models
{
    public class NhaXuatBan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nhà xuất bản không được để trống")]
        [StringLength(200)]
        public string TenNhaXuatBan { get; set; } = string.Empty;

        public string? DiaChi { get; set; }

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public ICollection<Sach>? Saches { get; set; }
    }
}
