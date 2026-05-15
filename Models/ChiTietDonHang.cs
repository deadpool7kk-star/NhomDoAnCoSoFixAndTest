using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class ChiTietDonHang
    {
        public int Id { get; set; }

        public int DonHangId { get; set; }
        public DonHang? DonHang { get; set; }

        public int SachId { get; set; }
        public Sach? Sach { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }
    }
}