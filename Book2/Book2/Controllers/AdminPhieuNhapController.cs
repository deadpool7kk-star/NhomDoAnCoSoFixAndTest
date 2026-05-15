using Book2.Data;
using Book2.Models;
using Book2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminPhieuNhapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminPhieuNhapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ds = await _context.PhieuNhaps
                .Include(x => x.NhaXuatBan)
                .OrderByDescending(x => x.NgayNhap)
                .ToListAsync();
            return View(ds);
        }

        public async Task<IActionResult> Details(int id)
        {
            var pn = await _context.PhieuNhaps
                .Include(x => x.NhaXuatBan)
                .Include(x => x.ChiTietPhieuNhaps)
                .ThenInclude(x => x.Sach)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pn == null) return NotFound();

            return View(pn);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDataAsync();
            return View(new PhieuNhapVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhieuNhapVM model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Vui lòng thêm ít nhất một sản phẩm.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var phieuNhap = new PhieuNhap
                    {
                        NhaXuatBanId = model.NhaXuatBanId,
                        NgayNhap = DateTime.Now,
                        GhiChu = model.GhiChu,
                        TongTien = model.Items!.Sum(x => x.SoLuong * x.GiaNhap)
                    };

                    _context.PhieuNhaps.Add(phieuNhap);
                    await _context.SaveChangesAsync();

                    foreach (var item in model.Items!)
                    {
                        var chiTiet = new ChiTietPhieuNhap
                        {
                            PhieuNhapId = phieuNhap.Id,
                            SachId = item.SachId,
                            SoLuong = item.SoLuong,
                            GiaNhap = item.GiaNhap
                        };
                        _context.ChiTietPhieuNhaps.Add(chiTiet);

                        // Cập nhật tồn kho và giá nhập mới nhất cho sách
                        var sach = await _context.Saches.FindAsync(item.SachId);
                        if (sach != null)
                        {
                            sach.SoLuong += item.SoLuong;
                            sach.GiaNhap = item.GiaNhap;
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Tạo phiếu nhập thành công và đã cập nhật tồn kho.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu phiếu nhập.");
                }
            }

            await LoadDataAsync();
            return View(model);
        }

        private async Task LoadDataAsync()
        {
            ViewBag.NhaXuatBanList = await _context.NhaXuatBans
                .OrderBy(x => x.TenNhaXuatBan)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TenNhaXuatBan })
                .ToListAsync();

            ViewBag.SachList = await _context.Saches
                .OrderBy(x => x.TenSach)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.TenSach })
                .ToListAsync();
        }
    }
}
