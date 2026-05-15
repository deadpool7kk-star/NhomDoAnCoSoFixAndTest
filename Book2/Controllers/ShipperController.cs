using Book2.Data;
using Book2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Shipper")]
    public class ShipperController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipperController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var model = new AdminDashboardVM
            {
                TongDonHang = await _context.DonHangs.CountAsync(),
                DonChoXacNhan = await _context.DonHangs.CountAsync(x => x.TrangThai == "Chờ xác nhận"),
                DonDangGiao = await _context.DonHangs.CountAsync(x => x.TrangThai == "Đang giao"),
                DonHoanThanh = await _context.DonHangs.CountAsync(x => x.TrangThai == "Hoàn thành"),
                DonDaHuy = await _context.DonHangs.CountAsync(x => x.TrangThai == "Đã hủy")
            };

            return View(model);
        }
    }
}