using Book2.Models;

namespace Book2.ViewModels
{
    public class AdminDashboardVM
    {
        public int TongSach { get; set; }
        public int TongTheLoai { get; set; }
        public int TongDonHang { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal TongGiaVon { get; set; }
        public decimal LoiNhuanRong => TongDoanhThu - TongGiaVon;

        public int DonChoXacNhan { get; set; }
        public int DonDangGiao { get; set; }
        public int DonHoanThanh { get; set; }
        public int DonDaHuy { get; set; }
        public int TongKhachHang { get; set; }

        public List<Sach> SachSapHetHang { get; set; } = new();
    }
}