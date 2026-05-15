using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminTheLoaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminTheLoaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ds = await _context.TheLoais
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(ds);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TheLoai model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.TheLoais.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm thể loại thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null) return NotFound();

            return View(theLoai);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TheLoai model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.TheLoais.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thể loại thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null) return NotFound();

            bool dangDuocDung = await _context.Saches.AnyAsync(x => x.TheLoaiId == id);
            if (dangDuocDung)
            {
                TempData["Error"] = "Không thể xóa vì thể loại đang được dùng trong sách.";
                return RedirectToAction(nameof(Index));
            }

            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa thể loại thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}