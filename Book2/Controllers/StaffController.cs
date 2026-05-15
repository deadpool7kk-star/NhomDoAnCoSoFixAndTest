using Book2.Data;
using Book2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardVM
            {
                TongSach = await _context.Saches.CountAsync(),
                TongTheLoai = await _context.TheLoais.CountAsync(),
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonChoXacNhan = await _context.DonHangs.CountAsync(x => x.TrangThai == "Chờ xác nhận"),
                DonDangGiao = await _context.DonHangs.CountAsync(x => x.TrangThai == "Đang giao"),
                DonHoanThanh = await _context.DonHangs.CountAsync(x => x.TrangThai == "Hoàn thành"),
                DonDaHuy = await _context.DonHangs.CountAsync(x => x.TrangThai == "Đã hủy"),
                SachSapHetHang = await _context.Saches
                    .Where(x => x.SoLuong < 5)
                    .OrderBy(x => x.SoLuong)
                    .Take(10)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}