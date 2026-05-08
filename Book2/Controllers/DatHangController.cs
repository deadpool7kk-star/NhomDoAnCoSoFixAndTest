using Book2.Data;
using Book2.Models;
using Book2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Book2.Controllers
{
    [Authorize]
    public class DatHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Không xác định được người dùng.");
            }

            return userId;
        }

        private async Task<GioHang?> GetGioHangAsync()
        {
            var userId = GetUserId();

            return await _context.GioHangs
                .Include(x => x.Coupon)
                .Include(x => x.ChiTietGioHangs)!
                .ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        [HttpGet]
        public async Task<IActionResult> ThanhToan()
        {
            var gioHang = await GetGioHangAsync();

            if (gioHang == null || gioHang.ChiTietGioHangs == null || !gioHang.ChiTietGioHangs.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var model = new ThanhToanVM
            {
                GioHangItems = gioHang.ChiTietGioHangs
                    .Where(x => x.Sach != null)
                    .Select(x => new GioHangItemVM
                    {
                        ChiTietGioHangId = x.Id,
                        SachId = x.SachId,
                        TenSach = x.Sach!.TenSach,
                        TacGia = x.Sach.TacGia,
                        Gia = x.Sach.Gia,
                        SoLuong = x.SoLuong,
                        HinhAnh = x.Sach.HinhAnh
                    })
                    .ToList(),
                CouponCode = gioHang.Coupon?.Code,
                PhanTramGiam = gioHang.Coupon?.PhanTramGiam
            };

            model.TongTienGoc = model.GioHangItems.Sum(x => x.ThanhTien);
            model.SoTienGiam = model.PhanTramGiam.HasValue ? (model.TongTienGoc * (decimal)model.PhanTramGiam.Value / 100) : 0;
            model.TongThanhToan = model.TongTienGoc - model.SoTienGiam;

            ViewBag.MuaNgay = false;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(ThanhToanVM model)
        {
            var gioHang = await GetGioHangAsync();

            if (gioHang == null || gioHang.ChiTietGioHangs == null || !gioHang.ChiTietGioHangs.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index", "GioHang");
            }

            var gioHangItems = gioHang.ChiTietGioHangs
                .Where(x => x.Sach != null)
                .Select(x => new GioHangItemVM
                {
                    ChiTietGioHangId = x.Id,
                    SachId = x.SachId,
                    TenSach = x.Sach!.TenSach,
                    TacGia = x.Sach.TacGia,
                    Gia = x.Sach.Gia,
                    SoLuong = x.SoLuong,
                    HinhAnh = x.Sach.HinhAnh
                })
                .ToList();

            model.GioHangItems = gioHangItems;
            model.TongTienGoc = gioHangItems.Sum(x => x.ThanhTien);
            model.CouponCode = gioHang.Coupon?.Code;
            model.PhanTramGiam = gioHang.Coupon?.PhanTramGiam;
            model.SoTienGiam = model.PhanTramGiam.HasValue ? (model.TongTienGoc * (decimal)model.PhanTramGiam.Value / 100) : 0;
            model.TongThanhToan = model.TongTienGoc - model.SoTienGiam;

            ViewBag.MuaNgay = false;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            foreach (var item in gioHang.ChiTietGioHangs)
            {
                if (item.Sach == null)
                {
                    TempData["Error"] = "Có sản phẩm không tồn tại trong giỏ hàng.";
                    return RedirectToAction("Index", "GioHang");
                }

                if (item.SoLuong > item.Sach.SoLuong)
                {
                    TempData["Error"] = $"Sách '{item.Sach.TenSach}' không đủ số lượng tồn kho.";
                    return RedirectToAction("Index", "GioHang");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var donHang = new DonHang
                {
                    UserId = GetUserId(),
                    TenNguoiNhan = model.TenNguoiNhan,
                    SoDienThoai = model.SoDienThoai,
                    DiaChiNhanHang = model.DiaChiNhanHang,
                    NgayDat = DateTime.Now,
                    TongTien = model.TongThanhToan,
                    SoTienGiam = model.SoTienGiam,
                    CouponCode = model.CouponCode,
                    TrangThai = "Chờ xác nhận"
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                foreach (var item in gioHang.ChiTietGioHangs)
                {
                    var sach = item.Sach!;

                    _context.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        DonHangId = donHang.Id,
                        SachId = item.SachId,
                        SoLuong = item.SoLuong,
                        DonGia = sach.Gia,
                        GiaNhap = sach.GiaNhap
                    });

                    sach.SoLuong -= item.SoLuong;
                }

                // Cập nhật số lượng coupon nếu có
                if (gioHang.Coupon != null)
                {
                    gioHang.Coupon.SoLuong -= 1;
                }

                _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
                gioHang.CouponId = null; // Xóa mã giảm giá sau khi dùng

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Đặt hàng thành công.";
                return RedirectToAction("DatHangThanhCong");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra khi đặt hàng.";
                return RedirectToAction("Index", "GioHang");
            }
        }

        [HttpGet]
        public async Task<IActionResult> MuaNgay(int sachId, int soLuong = 1)
        {
            var sach = await _context.Saches
                .Include(x => x.TheLoai)
                .FirstOrDefaultAsync(x => x.Id == sachId);

            if (sach == null)
            {
                return NotFound();
            }

            if (soLuong <= 0)
            {
                soLuong = 1;
            }

            if (sach.SoLuong <= 0)
            {
                TempData["Error"] = "Sách đã hết hàng.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            if (soLuong > sach.SoLuong)
            {
                TempData["Error"] = "Số lượng mua vượt quá tồn kho.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            var model = new ThanhToanVM
            {
                GioHangItems = new List<GioHangItemVM>
                {
                    new GioHangItemVM
                    {
                        SachId = sach.Id,
                        TenSach = sach.TenSach,
                        TacGia = sach.TacGia,
                        Gia = sach.Gia,
                        SoLuong = soLuong,
                        HinhAnh = sach.HinhAnh
                    }
                }
            };

            model.TongTienGoc = model.GioHangItems.Sum(x => x.ThanhTien);
            model.TongThanhToan = model.TongTienGoc;

            ViewBag.MuaNgay = true;
            ViewBag.SachId = sachId;
            ViewBag.SoLuong = soLuong;

            return View("ThanhToan", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MuaNgay(ThanhToanVM model, int sachId, int soLuong = 1)
        {
            var sach = await _context.Saches.FirstOrDefaultAsync(x => x.Id == sachId);

            if (sach == null)
            {
                return NotFound();
            }

            if (soLuong <= 0)
            {
                soLuong = 1;
            }

            model.GioHangItems = new List<GioHangItemVM>
            {
                new GioHangItemVM
                {
                    SachId = sach.Id,
                    TenSach = sach.TenSach,
                    TacGia = sach.TacGia,
                    Gia = sach.Gia,
                    SoLuong = soLuong,
                    HinhAnh = sach.HinhAnh
                }
            };

            model.TongTienGoc = model.GioHangItems.Sum(x => x.ThanhTien);
            model.TongThanhToan = model.TongTienGoc;

            ViewBag.MuaNgay = true;
            ViewBag.SachId = sachId;
            ViewBag.SoLuong = soLuong;

            if (!ModelState.IsValid)
            {
                return View("ThanhToan", model);
            }

            if (sach.SoLuong <= 0)
            {
                TempData["Error"] = "Sách đã hết hàng.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            if (soLuong > sach.SoLuong)
            {
                TempData["Error"] = "Số lượng mua vượt quá tồn kho.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var donHang = new DonHang
                {
                    UserId = GetUserId(),
                    TenNguoiNhan = model.TenNguoiNhan,
                    SoDienThoai = model.SoDienThoai,
                    DiaChiNhanHang = model.DiaChiNhanHang,
                    NgayDat = DateTime.Now,
                    TongTien = model.TongThanhToan,
                    TrangThai = "Chờ xác nhận"
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    DonHangId = donHang.Id,
                    SachId = sach.Id,
                    SoLuong = soLuong,
                    DonGia = sach.Gia,
                    GiaNhap = sach.GiaNhap
                });

                sach.SoLuong -= soLuong;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Mua ngay thành công.";
                return RedirectToAction("DatHangThanhCong");
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra khi mua ngay.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }
        }

        public IActionResult DatHangThanhCong()
        {
            return View();
        }

        public async Task<IActionResult> LichSu()
        {
            var userId = GetUserId();

            var dsDonHang = await _context.DonHangs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();

            return View(dsDonHang);
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var userId = GetUserId();

            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (donHang == null)
            {
                return NotFound();
            }

            var chiTiet = await _context.ChiTietDonHangs
                .Include(x => x.Sach)
                .Where(x => x.DonHangId == id)
                .ToListAsync();

            ViewBag.DonHang = donHang;
            return View(chiTiet);
        }
    }
}