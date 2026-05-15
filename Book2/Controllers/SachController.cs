using Book2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Book2.Controllers
{
    public class SachController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SachController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> ChiTiet(int id)
        {
            var sach = await _context.Saches
                .Include(x => x.TheLoai)
                .Include(x => x.NhaXuatBan)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (sach == null)
            {
                return NotFound();
            }

            var sachLienQuan = await _context.Saches
                .Include(x => x.TheLoai)
                .Where(x => x.TheLoaiId == sach.TheLoaiId && x.Id != sach.Id)
                .OrderByDescending(x => x.Id)
                .Take(4)
                .ToListAsync();

            var dsAnhDocThu = new List<string>();
            var thuMucDocThu = Path.Combine(_env.WebRootPath, "preview-books", id.ToString());

            if (Directory.Exists(thuMucDocThu))
            {
                var extensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                dsAnhDocThu = Directory.GetFiles(thuMucDocThu)
                    .Where(f => extensions.Contains(Path.GetExtension(f).ToLower()))
                    .OrderBy(f => LaySoThuTu(Path.GetFileNameWithoutExtension(f)))
                    .Select(f => $"/preview-books/{id}/{Path.GetFileName(f)}")
                    .ToList();
            }

            ViewBag.SachLienQuan = sachLienQuan;
            ViewBag.AnhDocThu = dsAnhDocThu;

            return View(sach);
        }

        private int LaySoThuTu(string tenFile)
        {
            var match = Regex.Match(tenFile, @"\d+");
            if (match.Success && int.TryParse(match.Value, out int so))
            {
                return so;
            }

            return int.MaxValue;
        }
    }
}