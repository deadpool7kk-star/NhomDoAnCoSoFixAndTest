using Book2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin,Staff,Shipper")]
    public class AdminDonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> InHoaDon(int id)
        {
            var donHang = await _context.DonHangs.FirstOrDefaultAsync(x => x.Id == id);
            if (donHang == null) return NotFound();

            var chiTiet = await _context.ChiTietDonHangs
                .Include(x => x.Sach)
                .Where(x => x.DonHangId == id)
                .ToListAsync();

            ViewBag.DonHang = donHang;

            return new ViewAsPdf("Invoice", chiTiet)
            {
                FileName = $"HoaDon_{donHang.Id}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };
        }

        public async Task<IActionResult> Index(string? trangThai)
        {
            IQueryable<Models.DonHang> query = _context.DonHangs;

            if (User.IsInRole("Shipper"))
            {
                query = query.Where(x =>
                    x.TrangThai == "Chờ xác nhận" ||
                    x.TrangThai == "Đang giao");
            }

            if (!string.IsNullOrEmpty(trangThai))
            {
                query = query.Where(x => x.TrangThai == trangThai);
            }

            var ds = await query
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();

            ViewBag.CurrentTrangThai = trangThai;
            return View(ds);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var donHang = await _context.DonHangs.FirstOrDefaultAsync(x => x.Id == id);
            if (donHang == null) return NotFound();

            if (User.IsInRole("Shipper"))
            {
                if (donHang.TrangThai != "Chờ xác nhận" && donHang.TrangThai != "Đang giao" && donHang.TrangThai != "Hoàn thành")
                {
                    TempData["Error"] = "Bạn không có quyền xem đơn này.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var chiTiet = await _context.ChiTietDonHangs
                .Include(x => x.Sach)
                .Where(x => x.DonHangId == id)
                .ToListAsync();

            List<string> trangThaiList;

            if (User.IsInRole("Admin"))
            {
                trangThaiList = new List<string>
                {
                    "Chờ xác nhận",
                    "Đang giao",
                    "Hoàn thành",
                    "Đã hủy"
                };
            }
            else if (User.IsInRole("Staff"))
            {
                trangThaiList = new List<string>
                {
                    "Chờ xác nhận",
                    "Đang giao",
                    "Đã hủy"
                };
            }
            else
            {
                trangThaiList = new List<string>
                {
                    "Đang giao",
                    "Hoàn thành"
                };
            }

            ViewBag.DonHang = donHang;
            ViewBag.TrangThaiList = trangThaiList;

            return View(chiTiet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string trangThai)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null) return NotFound();

            var validStatuses = new List<string>
            {
                "Chờ xác nhận",
                "Đang giao",
                "Hoàn thành",
                "Đã hủy"
            };

            if (!validStatuses.Contains(trangThai))
            {
                TempData["Error"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            if (donHang.TrangThai == "Hoàn thành" || donHang.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Đơn hàng đã kết thúc, không thể cập nhật nữa.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            if (User.IsInRole("Shipper"))
            {
                if (trangThai != "Đang giao" && trangThai != "Hoàn thành")
                {
                    TempData["Error"] = "Shipper chỉ được cập nhật sang Đang giao hoặc Hoàn thành.";
                    return RedirectToAction(nameof(Detail), new { id });
                }
            }

            if (User.IsInRole("Staff"))
            {
                if (trangThai == "Hoàn thành")
                {
                    TempData["Error"] = "Staff không được cập nhật đơn sang Hoàn thành.";
                    return RedirectToAction(nameof(Detail), new { id });
                }
            }

            donHang.TrangThai = trangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công.";
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}