namespace Book2.ViewModels
{
    public class GioHangItemVM
    {
        public int ChiTietGioHangId { get; set; }
        public int SachId { get; set; }

        public string TenSach { get; set; } = string.Empty;
        public string TacGia { get; set; } = string.Empty;

        public decimal Gia { get; set; }
        public int SoLuong { get; set; }

        public string? HinhAnh { get; set; }

        public decimal ThanhTien => Gia * SoLuong;
    }
}