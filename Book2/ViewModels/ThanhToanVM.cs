using System.ComponentModel.DataAnnotations;

namespace Book2.ViewModels
{
    public class ThanhToanVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận")]
        [StringLength(100, ErrorMessage = "Tên người nhận tối đa 100 ký tự")]
        public string TenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [RegularExpression(@"^(0[0-9]{9,10})$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng")]
        [StringLength(250, ErrorMessage = "Địa chỉ nhận hàng tối đa 250 ký tự")]
        public string DiaChiNhanHang { get; set; } = string.Empty;

        public List<GioHangItemVM> GioHangItems { get; set; } = new();

        public decimal TongTienGoc { get; set; }
        public string? CouponCode { get; set; }
        public int? PhanTramGiam { get; set; }
        public decimal SoTienGiam { get; set; }
        public decimal TongThanhToan { get; set; }
    }
}