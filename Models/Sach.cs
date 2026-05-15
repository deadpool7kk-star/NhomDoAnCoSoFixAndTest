using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class Sach
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sách không được để trống")]
        [StringLength(200)]
        public string TenSach { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tác giả không được để trống")]
        [StringLength(150)]
        public string TacGia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaNhap { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaGoc { get; set; }

        public int? PhanTramGiam { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        public int SoLuong { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool NoiBat { get; set; } = false;

        public int? NamXuatBan { get; set; }

        public string? NgonNgu { get; set; }

        public int? SoTrang { get; set; }

        public int? TacGiaId { get; set; }
        public TacGia? TacGiaObj { get; set; }

        public int? NhaXuatBanId { get; set; }
        public NhaXuatBan? NhaXuatBan { get; set; }

        [Required]
        public int TheLoaiId { get; set; }

        public TheLoai? TheLoai { get; set; }
    }
}