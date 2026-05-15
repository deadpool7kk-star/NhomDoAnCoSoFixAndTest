using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class DonHang
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [StringLength(250)]
        public string DiaChiNhanHang { get; set; } = string.Empty;

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public string? CouponCode { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTienGiam { get; set; }

        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận";

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}