using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class PhieuNhap
    {
        public int Id { get; set; }

        [Required]
        public DateTime NgayNhap { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int NhaXuatBanId { get; set; }
        public NhaXuatBan? NhaXuatBan { get; set; }

        public string? GhiChu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
    }
}
