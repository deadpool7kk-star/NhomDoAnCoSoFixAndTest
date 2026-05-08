namespace Book2.ViewModels
{
    public class CartVM
    {
        public List<GioHangItemVM> Items { get; set; } = new();
        public string? CouponCode { get; set; }
        public int? DiscountPercentage { get; set; }
        
        public decimal TongTienChuaGiam => Items.Sum(x => x.ThanhTien);
        public decimal SoTienGiam => DiscountPercentage.HasValue ? (TongTienChuaGiam * (decimal)DiscountPercentage.Value / 100) : 0;
        public decimal TongThanhToan => TongTienChuaGiam - SoTienGiam;
    }
}
