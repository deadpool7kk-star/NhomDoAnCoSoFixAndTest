using System.ComponentModel.DataAnnotations;

namespace Book2.Models
{
    public class TheLoai
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thể loại không được để trống")]
        [StringLength(100)]
        public string TenTheLoai { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public ICollection<Sach> Saches { get; set; } = new List<Sach>();
    }
}