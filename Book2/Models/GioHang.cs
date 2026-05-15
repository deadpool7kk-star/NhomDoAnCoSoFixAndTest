namespace Book2.Models
{
    public class GioHang
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public int? CouponId { get; set; }
        public Coupon? Coupon { get; set; }

        public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
    }
}