using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book2.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã giảm giá không được để trống")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phần trăm giảm không được để trống")]
        public int PhanTramGiam { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        public int SoLuong { get; set; }

        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        public DateTime NgayHetHan { get; set; } = DateTime.Now.AddDays(7);

        public bool IsActive { get; set; } = true;
    }
}
