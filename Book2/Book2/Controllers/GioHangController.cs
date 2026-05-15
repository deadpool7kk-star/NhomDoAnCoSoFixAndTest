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
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        private async Task<GioHang> GetOrCreateGioHangAsync()
        {
            var userId = GetUserId();

            var gioHang = await _context.GioHangs
                .Include(x => x.Coupon)
                .Include(x => x.ChiTietGioHangs)!
                .ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    UserId = userId
                };

                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();

                gioHang = await _context.GioHangs
                    .Include(x => x.Coupon)
                    .Include(x => x.ChiTietGioHangs)!
                    .ThenInclude(x => x.Sach)
                    .FirstAsync(x => x.UserId == userId);
            }

            return gioHang;
        }

        public async Task<IActionResult> Index()
        {
            var gioHang = await GetOrCreateGioHangAsync();

            var model = new CartVM
            {
                Items = gioHang.ChiTietGioHangs?
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
                    .ToList() ?? new List<GioHangItemVM>(),
                CouponCode = gioHang.Coupon?.Code,
                DiscountPercentage = gioHang.Coupon?.PhanTramGiam
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var gioHang = await GetOrCreateGioHangAsync();

            if (string.IsNullOrEmpty(couponCode))
            {
                gioHang.CouponId = null;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã bỏ áp dụng mã giảm giá.";
                return RedirectToAction("Index");
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(x => x.Code == couponCode && x.IsActive && x.NgayHetHan >= DateTime.Now && x.NgayBatDau <= DateTime.Now);

            if (coupon == null)
            {
                TempData["Error"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Index");
            }

            if (coupon.SoLuong <= 0)
            {
                TempData["Error"] = "Mã giảm giá đã hết lượt sử dụng.";
                return RedirectToAction("Index");
            }

            gioHang.CouponId = coupon.Id;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Áp dụng mã giảm giá {coupon.Code} thành công! Bạn được giảm {coupon.PhanTramGiam}%.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemVaoGio(int sachId, int soLuong = 1)
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

            if (sach.SoLuong <= 0)
            {
                TempData["Error"] = "Sách đã hết hàng.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            if (soLuong > sach.SoLuong)
            {
                TempData["Error"] = "Số lượng thêm vào vượt quá tồn kho.";
                return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
            }

            var gioHang = await GetOrCreateGioHangAsync();

            var item = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(x => x.GioHangId == gioHang.Id && x.SachId == sachId);

            if (item == null)
            {
                item = new ChiTietGioHang
                {
                    GioHangId = gioHang.Id,
                    SachId = sachId,
                    SoLuong = soLuong
                };

                _context.ChiTietGioHangs.Add(item);
            }
            else
            {
                int tongSoLuongMoi = item.SoLuong + soLuong;

                if (tongSoLuongMoi > sach.SoLuong)
                {
                    TempData["Error"] = "Tổng số lượng trong giỏ vượt quá số lượng tồn kho.";
                    return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
                }

                item.SoLuong = tongSoLuongMoi;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã thêm sách vào giỏ hàng.";
            return RedirectToAction("ChiTiet", "Sach", new { id = sachId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatSoLuong(int chiTietGioHangId, int soLuong)
        {
            var item = await _context.ChiTietGioHangs
                .Include(x => x.Sach)
                .Include(x => x.GioHang)
                .FirstOrDefaultAsync(x => x.Id == chiTietGioHangId);

            if (item == null || item.GioHang == null || item.GioHang.UserId != GetUserId())
            {
                return NotFound();
            }

            if (soLuong <= 0)
            {
                _context.ChiTietGioHangs.Remove(item);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
                return RedirectToAction("Index");
            }

            if (item.Sach == null)
            {
                TempData["Error"] = "Không tìm thấy sách.";
                return RedirectToAction("Index");
            }

            if (soLuong > item.Sach.SoLuong)
            {
                TempData["Error"] = "Số lượng cập nhật vượt quá tồn kho.";
                return RedirectToAction("Index");
            }

            item.SoLuong = soLuong;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật giỏ hàng thành công.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int chiTietGioHangId)
        {
            var item = await _context.ChiTietGioHangs
                .Include(x => x.GioHang)
                .FirstOrDefaultAsync(x => x.Id == chiTietGioHangId);

            if (item == null || item.GioHang == null || item.GioHang.UserId != GetUserId())
            {
                return NotFound();
            }

            _context.ChiTietGioHangs.Remove(item);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaTatCa()
        {
            var gioHang = await GetOrCreateGioHangAsync();

            if (gioHang.ChiTietGioHangs != null && gioHang.ChiTietGioHangs.Any())
            {
                _context.ChiTietGioHangs.RemoveRange(gioHang.ChiTietGioHangs);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng.";
            return RedirectToAction("Index");
        }
    }
}