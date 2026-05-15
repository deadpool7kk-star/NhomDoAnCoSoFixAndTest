using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class ChiTietPhieuNhap
    {
        public int Id { get; set; }

        public int PhieuNhapId { get; set; }
        public PhieuNhap? PhieuNhap { get; set; }

        public int SachId { get; set; }
        public Sach? Sach { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        public decimal ThanhTien => SoLuong * GiaNhap;
    }
}
