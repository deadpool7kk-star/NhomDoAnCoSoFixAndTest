namespace Book2.ViewModels
{
    public class PhieuNhapVM
    {
        public int NhaXuatBanId { get; set; }
        public string? GhiChu { get; set; }
        public List<ChiTietPhieuNhapVM> Items { get; set; } = new();
    }

    public class ChiTietPhieuNhapVM
    {
        public int SachId { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaNhap { get; set; }
    }
}
