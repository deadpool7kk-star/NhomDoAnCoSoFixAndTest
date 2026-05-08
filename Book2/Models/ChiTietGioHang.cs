namespace Book2.Models
{
    public class ChiTietGioHang
    {
        public int Id { get; set; }

        public int GioHangId { get; set; }
        public GioHang? GioHang { get; set; }

        public int SachId { get; set; }
        public Sach? Sach { get; set; }

        public int SoLuong { get; set; }
    }
}