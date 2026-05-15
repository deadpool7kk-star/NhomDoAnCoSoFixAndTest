using System.ComponentModel.DataAnnotations;

namespace Book2.Models
{
    public class TacGia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [StringLength(200)]
        public string TenTacGia { get; set; } = string.Empty;

        public string? TieuSu { get; set; }

        public string? HinhAnh { get; set; }

        public ICollection<Sach>? Saches { get; set; }
    }
}
