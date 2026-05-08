using Book2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Book2.Utils;

namespace Book2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? tuKhoa, int? theLoaiId, string? sapXep)
        {
            var query = _context.Saches
                .Include(x => x.TheLoai)
                .Where(x => x.SoLuong > 0)
                .AsQueryable();

            bool isSearchMode = false;

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                isSearchMode = true;
                // We will perform the search in-memory using SmartSearch later
            }

            if (theLoaiId.HasValue)
            {
                query = query.Where(x => x.TheLoaiId == theLoaiId.Value);
                isSearchMode = true;

                var theLoai = await _context.TheLoais.FindAsync(theLoaiId.Value);
                if (theLoai != null)
                {
                    ViewBag.TenTheLoai = theLoai.TenTheLoai;
                }
            }

            if (!string.IsNullOrEmpty(sapXep))
            {
                isSearchMode = true;
            }

            // Only apply default sorting in database if we are not doing a smart search.
            // If we are searching by keyword, we want to fetch all and then rank by relevance score.
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                query = sapXep switch
                {
                    "gia_tang" => query.OrderBy(x => x.Gia),
                    "gia_giam" => query.OrderByDescending(x => x.Gia),
                    "ten_az" => query.OrderBy(x => x.TenSach),
                    _ => query.OrderByDescending(x => x.NgayTao)
                };
            }

            var dsSach = await query.ToListAsync();

            // Apply Smart Search in memory
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                dsSach = SearchHelper.SmartSearch(dsSach, tuKhoa).ToList();

                // If user selected a specific sort order, override the relevance ranking
                if (!string.IsNullOrEmpty(sapXep))
                {
                    dsSach = sapXep switch
                    {
                        "gia_tang" => dsSach.OrderBy(x => x.Gia).ToList(),
                        "gia_giam" => dsSach.OrderByDescending(x => x.Gia).ToList(),
                        "ten_az" => dsSach.OrderBy(x => x.TenSach).ToList(),
                        _ => dsSach
                    };
                }
            }

            ViewBag.TuKhoa = tuKhoa;
            ViewBag.TheLoaiId = theLoaiId;
            ViewBag.SapXep = sapXep;

            if (isSearchMode)
            {
                return View("Search", dsSach);
            }

            return View("LandingPage", dsSach);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new List<object>());
            }

            var allBooks = await _context.Saches
                .Include(x => x.TheLoai)
                .Where(x => x.SoLuong > 0)
                .ToListAsync();

            var results = SearchHelper.SmartSearch(allBooks, query)
                .Take(5)
                .Select(x => new
                {
                    id = x.Id,
                    tenSach = x.TenSach,
                    tacGia = x.TacGia,
                    hinhAnh = x.HinhAnh ?? "/images/no-image.png",
                    gia = x.Gia.ToString("N0") + " đ"
                });

            return Json(results);
        }
    }
}