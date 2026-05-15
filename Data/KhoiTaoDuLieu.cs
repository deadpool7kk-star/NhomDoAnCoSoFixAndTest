using Book2.Models;

namespace Book2.Data
{
    public static class KhoiTaoDuLieu
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.TheLoais.Any() || context.Saches.Any())
            {
                return;
            }

            var theLoais = new List<TheLoai>
            {
                new TheLoai
                {
                    TenTheLoai = "Văn học",
                    MoTa = "Các tác phẩm văn học trong và ngoài nước"
                },
                new TheLoai
                {
                    TenTheLoai = "Kinh tế",
                    MoTa = "Sách về kinh doanh, tài chính, quản trị"
                },
                new TheLoai
                {
                    TenTheLoai = "Kỹ năng sống",
                    MoTa = "Sách giúp phát triển bản thân và kỹ năng mềm"
                },
                new TheLoai
                {
                    TenTheLoai = "Công nghệ thông tin",
                    MoTa = "Sách lập trình, công nghệ, dữ liệu"
                },
                new TheLoai
                {
                    TenTheLoai = "Thiếu nhi",
                    MoTa = "Sách dành cho trẻ em"
                }
            };

            context.TheLoais.AddRange(theLoais);
            context.SaveChanges();

            var sachList = new List<Sach>
            {
                new Sach
                {
                    TenSach = "Nhà Giả Kim",
                    TacGia = "Paulo Coelho",
                    Gia = 79000,
                    GiaGoc = 99000,
                    PhanTramGiam = 20,
                    SoLuong = 20,
                    NamXuatBan = 2020,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 227,
                    MoTa = "NHÀ GIẢ KIM - HÀNH TRÌNH ĐI TÌM KHO BÁU HAY CUỘC HÀNH TRÌNH TÌM KIẾM CHÍNH MÌNH\n\"Nhà Giả Kim\" không đơn thuần là một cuốn tiểu thuyết, mà là bản đồ dẫn lối đến giấc mơ, khao khát và định mệnh của mỗi con người. Câu chuyện về chàng trai chăn cừu Santiago không chỉ mang đến những cuộc phiêu lưu hấp dẫn, mà còn mở ra nhiều tầng triết lý sâu sắc về cuộc sống.\n\nVỀ TÁC GIẢ:Paulo Coelho \nLà nhà văn người Brazil, bậc thầy kể chuyện với lối viết đậm chất triết lý. \n\nÔng là tác giả của nhiều tác phẩm truyền cảm hứng, trong đó \"Nhà Giả Kim\" là cuốn sách nổi tiếng nhất, được dịch ra hơn 80 ngôn ngữ và bán hàng triệu bản trên toàn thế giới. \n\nCác tác phẩm khác của ông như \"Veronika quyết chết\", \"Nhà tiên tri\" hay \"Phù thủy thành phố Portobello\" cũng để lại dấu ấn sâu sắc trong lòng độc giả.\n\nVỀ DỊCH GIẢ:Lê Chu Cầu\nLà dịch giả có nhiều đóng góp trong việc đưa văn học nước ngoài đến với độc giả Việt Nam.\n\nÔng đã chuyển ngữ nhiều tác phẩm kinh điển, trong đó bản dịch \"Nhà Giả Kim\" của ông được đánh giá cao bởi sự mượt mà, giàu cảm xúc và giữ trọn vẹn tinh thần triết lý của Paulo Coelho.\n\nTÓM TẮT NỘI DUNG SÁCH\nSantiago – một chàng trai chăn cừu trẻ tuổi, rời quê hương để theo đuổi giấc mơ tìm kho báu ở Kim Tự Tháp Ai Cập. Trên hành trình ấy, anh gặp gỡ nhiều người, từ ông vua thông thái, người buôn pha lê đến Nhà Giả Kim huyền bí. Tất cả những trải nghiệm trong chuyến phiêu du theo đuổi vận mệnh của mình đã giúp Santiago thấu hiểu được ý nghĩa sâu xa nhất của hạnh phúc, hòa hợp với vũ trụ và con người.\n\nMỗi cuộc gặp gỡ không chỉ giúp anh tiến gần hơn đến kho báu mà còn giúp anh hiểu được mục đích thật sự của đời mình: \n\nTrích Nhà giả Kim: \"Kho báu không nằm ở nơi ta đến, mà nằm trong chính hành trình ta đi.\"\n\nQuyển sách mang đến cho độc giả:\nTruyền cảm hứng mạnh mẽ để theo đuổi đam mê, ước mơ. \n\nKhám phá những triết lý sâu sắc về cuộc sống và định mệnh. \n\nMột câu chuyện lôi cuốn, đầy tính phiêu lưu nhưng cũng giàu chất thơ và suy ngẫm.\n\nMột nguồn cảm hứng lớn lao, thể hiện sức mạnh của lòng trung thành và tình bạn. \n\nNhững bài học về lòng dũng cảm, sự hy sinh vì người khác sẽ khiến độc giả suy ngẫm và trân trọng hơn những giá trị cuộc sống.\n\nTại sao độc giả nên đọc?",
                    HinhAnh = "/images/Nha-gia-kim.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Văn học").Id,
                    NgayTao = DateTime.Now.AddDays(-5),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Tuổi Trẻ Đáng Giá Bao Nhiêu",
                    TacGia = "Rosie Nguyễn",
                    Gia = 85000,
                    GiaGoc = 100000,
                    PhanTramGiam = 15,
                    SoLuong = 15,
                    NamXuatBan = 2017,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 285,
                    MoTa = "Cuốn sách truyền cảm hứng cho người trẻ về học tập, trải nghiệm và trưởng thành.",
                    HinhAnh = "/images/Tuoi-tre-dang-gia-bao-nhieu.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Kỹ năng sống").Id,
                    NgayTao = DateTime.Now.AddDays(-3),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Đắc Nhân Tâm",
                    TacGia = "Dale Carnegie",
                    Gia = 92000,
                    GiaGoc = 110000,
                    PhanTramGiam = 16,
                    SoLuong = 25,
                    NamXuatBan = 2021,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 320,
                    MoTa = "Cuốn sách kinh điển về nghệ thuật giao tiếp và ứng xử.",
                    HinhAnh = "/images/Dac-nhan-tam.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Kỹ năng sống").Id,
                    NgayTao = DateTime.Now.AddDays(-10),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Lập Trình C# Cơ Bản",
                    TacGia = "Nguyễn Văn A",
                    Gia = 120000,
                    GiaGoc = 140000,
                    PhanTramGiam = 14,
                    SoLuong = 12,
                    NamXuatBan = 2023,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 450,
                    MoTa = "Sách dành cho người mới bắt đầu học lập trình C#.",
                    HinhAnh = "/images/lap-trinh-c-co-ban.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Công nghệ thông tin").Id,
                    NgayTao = DateTime.Now.AddDays(-1),
                    NoiBat = false
                },
                new Sach
                {
                    TenSach = "ASP.NET Core MVC Từ Cơ Bản Đến Nâng Cao",
                    TacGia = "Trần Minh Khang",
                    Gia = 145000,
                    GiaGoc = 170000,
                    PhanTramGiam = 15,
                    SoLuong = 10,
                    NamXuatBan = 2024,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 580,
                    MoTa = "Hướng dẫn xây dựng ứng dụng web với ASP.NET Core MVC.",
                    HinhAnh = "/images/Asp.net-core-co-ban.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Công nghệ thông tin").Id,
                    NgayTao = DateTime.Now.AddDays(-2),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Cha Giàu Cha Nghèo",
                    TacGia = "Robert Kiyosaki",
                    Gia = 99000,
                    GiaGoc = 119000,
                    PhanTramGiam = 17,
                    SoLuong = 18,
                    NamXuatBan = 2019,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 336,
                    MoTa = "Cuốn sách nổi tiếng về tư duy tài chính cá nhân.",
                    HinhAnh = "/images/Cha-giau-cha-ngheo.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Kinh tế").Id,
                    NgayTao = DateTime.Now.AddDays(-7),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Dạy Con Làm Giàu",
                    TacGia = "Robert Kiyosaki",
                    Gia = 105000,
                    GiaGoc = 125000,
                    PhanTramGiam = 16,
                    SoLuong = 14,
                    NamXuatBan = 2020,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 400,
                    MoTa = "Bộ sách hướng dẫn tư duy tài chính và đầu tư.",
                    HinhAnh = "/images/Day-con-lam-giau.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Kinh tế").Id,
                    NgayTao = DateTime.Now.AddDays(-4),
                    NoiBat = false
                },
                new Sach
                {
                    TenSach = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh",
                    TacGia = "Nguyễn Nhật Ánh",
                    Gia = 88000,
                    GiaGoc = 98000,
                    PhanTramGiam = 10,
                    SoLuong = 17,
                    NamXuatBan = 2010,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 378,
                    MoTa = "Tác phẩm nổi tiếng của Nguyễn Nhật Ánh về tuổi thơ và ký ức.",
                    HinhAnh = "/images/Toi-thay-hoa-vang-tren-co-xanh.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Văn học").Id,
                    NgayTao = DateTime.Now.AddDays(-6),
                    NoiBat = false
                },
                new Sach
                {
                    TenSach = "Cho Tôi Xin Một Vé Đi Tuổi Thơ",
                    TacGia = "Nguyễn Nhật Ánh",
                    Gia = 76000,
                    GiaGoc = 90000,
                    PhanTramGiam = 15,
                    SoLuong = 19,
                    NamXuatBan = 2008,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 220,
                    MoTa = "Câu chuyện nhẹ nhàng, sâu sắc về tuổi thơ.",
                    HinhAnh = "/images/Cho-toi-xin-mot-ve-di-tuoi-tho.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Văn học").Id,
                    NgayTao = DateTime.Now.AddDays(-8),
                    NoiBat = false
                },
                new Sach
                {
                    TenSach = "Dế Mèn Phiêu Lưu Ký",
                    TacGia = "Tô Hoài",
                    Gia = 65000,
                    GiaGoc = 80000,
                    PhanTramGiam = 19,
                    SoLuong = 30,
                    NamXuatBan = 2022,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 150,
                    MoTa = "Tác phẩm thiếu nhi kinh điển của văn học Việt Nam.",
                    HinhAnh = "/images/De-men-phieu-luu-ky.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Thiếu nhi").Id,
                    NgayTao = DateTime.Now.AddDays(-9),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Harry Potter Và Hòn Đá Phù Thủy",
                    TacGia = "J.K. Rowling",
                    Gia = 135000,
                    GiaGoc = 155000,
                    PhanTramGiam = 13,
                    SoLuong = 16,
                    NamXuatBan = 2018,
                    NgonNgu = "Tiếng Việt",
                    SoTrang = 400,
                    MoTa = "Phần đầu tiên trong series Harry Potter nổi tiếng.",
                    HinhAnh = "/images/Harry-potter-Va-hon-da-phu-thuy.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Thiếu nhi").Id,
                    NgayTao = DateTime.Now.AddDays(-11),
                    NoiBat = true
                },
                new Sach
                {
                    TenSach = "Clean Code",
                    TacGia = "Robert C. Martin",
                    Gia = 210000,
                    GiaGoc = 250000,
                    PhanTramGiam = 16,
                    SoLuong = 8,
                    NamXuatBan = 2008,
                    NgonNgu = "Tiếng Anh",
                    SoTrang = 464,
                    MoTa = "Cuốn sách kinh điển dành cho lập trình viên về viết mã sạch.",
                    HinhAnh = "/images/Clean-Code.jpg",
                    TheLoaiId = theLoais.First(x => x.TenTheLoai == "Công nghệ thông tin").Id,
                    NgayTao = DateTime.Now.AddDays(-12),
                    NoiBat = true
                }
            };

            context.Saches.AddRange(sachList);
            context.SaveChanges();
        }
    }
}