using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCouponController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coupon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mã giảm giá thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coupon);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật mã giảm giá thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CouponExists(coupon.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa mã giảm giá thành công";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}
