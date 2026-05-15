using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Book2.Services;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin,Staff")]  
    public class AdminSachController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogService _logService;

        public AdminSachController(ApplicationDbContext context, IWebHostEnvironment environment, ILogService logService)
        {
            _context = context;
            _environment = environment;
            _logService = logService;
        }

        public async Task<IActionResult> Index(string? tuKhoa)
        {
            var query = _context.Saches
                .Include(x => x.TheLoai)
                .Include(x => x.TacGiaObj)
                .Include(x => x.NhaXuatBan)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = query.Where(x =>
                    x.TenSach.Contains(tuKhoa) ||
                    x.TacGia.Contains(tuKhoa));
            }

            var ds = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            ViewBag.TuKhoa = tuKhoa;
            return View(ds);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDataAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sach model, IFormFile? hinhUpload)
        {
            if (ModelState.IsValid)
            {
                if (hinhUpload != null && hinhUpload.Length > 0)
                {
                    model.HinhAnh = await SaveImageAsync(hinhUpload);
                }

                _context.Saches.Add(model);
                await _context.SaveChangesAsync();

                await _logService.LogAsync("Create", "Sach", $"Thêm sách mới: {model.TenSach} (ID: {model.Id})", User, HttpContext.Connection.RemoteIpAddress?.ToString());

                TempData["Success"] = "Thêm sách thành công.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDataAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            await LoadDataAsync();
            return View(sach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Sach model, IFormFile? hinhUpload)
        {
            if (ModelState.IsValid)
            {
                var sachDb = await _context.Saches.FindAsync(model.Id);
                if (sachDb == null) return NotFound();

                sachDb.TenSach = model.TenSach;
                sachDb.TacGia = model.TacGia; // Vẫn giữ trường text cũ cho tương thích
                sachDb.TacGiaId = model.TacGiaId;
                sachDb.NhaXuatBanId = model.NhaXuatBanId;
                sachDb.Gia = model.Gia;
                sachDb.SoLuong = model.SoLuong;
                sachDb.MoTa = model.MoTa;
                sachDb.TheLoaiId = model.TheLoaiId;
                sachDb.NamXuatBan = model.NamXuatBan;
                sachDb.NgonNgu = model.NgonNgu;
                sachDb.SoTrang = model.SoTrang;

                if (hinhUpload != null && hinhUpload.Length > 0)
                {
                    sachDb.HinhAnh = await SaveImageAsync(hinhUpload);
                }

                await _context.SaveChangesAsync();

                await _logService.LogAsync("Update", "Sach", $"Cập nhật sách: {sachDb.TenSach} (ID: {sachDb.Id})", User, HttpContext.Connection.RemoteIpAddress?.ToString());

                TempData["Success"] = "Cập nhật sách thành công.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDataAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            bool daCoTrongDonHang = await _context.ChiTietDonHangs.AnyAsync(x => x.SachId == id);
            if (daCoTrongDonHang)
            {
                TempData["Error"] = "Không thể xóa sách vì sách đã tồn tại trong đơn hàng.";
                return RedirectToAction(nameof(Index));
            }

            _context.Saches.Remove(sach);
            await _context.SaveChangesAsync();

            await _logService.LogAsync("Delete", "Sach", $"Xóa sách: {sach.TenSach} (ID: {id})", User, HttpContext.Connection.RemoteIpAddress?.ToString());

            TempData["Success"] = "Xóa sách thành công.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDataAsync()
        {
            ViewBag.TheLoaiList = await _context.TheLoais
                .OrderBy(x => x.TenTheLoai)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TenTheLoai })
                .ToListAsync();

            ViewBag.TacGiaList = await _context.TacGias
                .OrderBy(x => x.TenTacGia)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TenTacGia })
                .ToListAsync();

            ViewBag.NhaXuatBanList = await _context.NhaXuatBans
                .OrderBy(x => x.TenNhaXuatBan)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TenNhaXuatBan })
                .ToListAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_environment.WebRootPath, "images");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/" + fileName;
        }
    }
}