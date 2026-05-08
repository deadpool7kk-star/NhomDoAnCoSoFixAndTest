using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminNhaXuatBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNhaXuatBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.NhaXuatBans.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaXuatBan nxb)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nxb);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm nhà xuất bản thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(nxb);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var nxb = await _context.NhaXuatBans.FindAsync(id);
            if (nxb == null) return NotFound();
            return View(nxb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaXuatBan nxb)
        {
            if (id != nxb.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nxb);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật nhà xuất bản thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhaXuatBanExists(nxb.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nxb);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var nxb = await _context.NhaXuatBans.FindAsync(id);
            if (nxb != null)
            {
                _context.NhaXuatBans.Remove(nxb);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa nhà xuất bản thành công";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NhaXuatBanExists(int id)
        {
            return _context.NhaXuatBans.Any(e => e.Id == id);
        }
    }
}
